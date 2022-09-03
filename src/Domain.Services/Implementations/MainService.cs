using Domain.DataAccess;
using Domain.DataAccess.Models;
using Domain.Services.Contracts;
using Domain.Shared.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Operator.Extensions;
using Operator.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain.Services.Implementations
{
    public class MainService : IMainService
    {
        private readonly IExcelGeneratorService _excelGenerator;
        private readonly IDistributedCache _distributedCache;
        private readonly IExcelReader<ExcelDto> _excelReader;
        private readonly IServiceProvider _serviceProvider;

        public MainService(IExcelGeneratorService excelGenerator,
            IExcelReader<ExcelDto> excelReader,
            IDistributedCache distributedCache,
            IServiceProvider serviceProvider)
        {
            _excelGenerator = excelGenerator;
            _excelReader = excelReader;
            _distributedCache = distributedCache;
            _serviceProvider = serviceProvider;
        }

        public async Task DoMainOperation()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Operation starts");

            Console.WriteLine("Step 1 : ");
            var filePath = await GenerateExcellFile();
            Console.WriteLine("---------------------------------------------------------------------------");

            Console.WriteLine("Step 2 : ");
            var objs1 = await ReadFromExcel(filePath);
            Console.WriteLine("---------------------------------------------------------------------------");

            Console.WriteLine("Step 3 : ");
            await SaveInRedis(filePath, objs1);
            Console.WriteLine("---------------------------------------------------------------------------");

            Console.WriteLine("Step 4 : ");
            var redisObjects = await ReadFromRedis(filePath);
            Console.WriteLine("---------------------------------------------------------------------------");

            Console.WriteLine("Step 5 : ");
            await SaveInDb(redisObjects.Distinct().ToList());
            Console.WriteLine("---------------------------------------------------------------------------");

            await _distributedCache.RemoveAsync(filePath.GetRedisKey());

            Console.WriteLine("Operation end");
            sw.Stop();
            Console.WriteLine($"operation completed in '{sw.ElapsedMilliseconds}' milli seconds");
        }

        private async Task<string> GenerateExcellFile()
        {
            Console.WriteLine($"Generate excell file with name of 'excel.xlsx' with 500_000 rows...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var objs = await ExcelUtilityHelper.GenerateAsync(500_000);
            var filePath = await _excelGenerator.Generate(objs, "excel.xlsx");
            sw.Stop();
            Console.WriteLine($"excel file generated in '{sw.ElapsedMilliseconds}' milli seconds");
            return filePath;
        }

        private async Task<IEnumerable<ExcelDto>> ReadFromExcel(string filePath)
        {
            Console.WriteLine($"read excel file from file ...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var excelObjects = await _excelReader.ReadFileToObjAsync(filePath, 500_000);
            sw.Stop();
            Console.WriteLine($"excel file read in '{sw.ElapsedMilliseconds}' milli seconds");
            return excelObjects;
        }

        private async Task SaveInRedis(string fileName, IEnumerable<ExcelDto> excelObjects)
        {
            Console.WriteLine($"Save the records with generated key in redis ...");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            excelObjects = excelObjects.Distinct().ToList();

            var bytes = excelObjects.SerializeToBinary();

            await _distributedCache.SetAsync(fileName.GetRedisKey(), bytes, options);

            sw.Stop();
            Console.WriteLine($"The recods saved in redis in '{sw.ElapsedMilliseconds}' milli seconds");
        }

        private async Task<IEnumerable<ExcelDto>> ReadFromRedis(string fileName)
        {
            Console.WriteLine($"Read the records with generated key in redis ...");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var redisResult = await _distributedCache.GetStringAsync(fileName.GetRedisKey());
            var objects = JsonSerializer.Deserialize<IEnumerable<ExcelDto>>(redisResult);

            sw.Stop();
            Console.WriteLine($"The recods were read from redis in '{sw.ElapsedMilliseconds}' milli seconds");
            return objects.Distinct().ToList();
        }

        private async Task SaveInDb(List<ExcelDto> records, int batchSize = 5000)
        {
            Console.WriteLine($"Start saving the records in database ...");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (records.Count() <= 0)
            {
                sw.Stop();
                Console.WriteLine($"The records are saved in db in '{sw.ElapsedMilliseconds}' milli seconds");
                return;
            }

            if (records.Count() <= batchSize)
            {
                await SaveInDbTask(records);
                sw.Stop();
                Console.WriteLine($"The records are saved in db in '{sw.ElapsedMilliseconds}' milli seconds");
            }

            await Task.WhenAll(CreateBatchSaveInDbTasks(records, batchSize));

            sw.Stop();
            Console.WriteLine($"The records are saved in db in '{sw.ElapsedMilliseconds}' milli seconds");
        }

        private ICollection<Task> CreateBatchSaveInDbTasks(List<ExcelDto> records, int batchSize = 5000)
        {
            ICollection<Task> saveInDbTasks = new Collection<Task>();

            var recordsList = records.Distinct().ToList().ChunkBy(batchSize);

            foreach (var list in recordsList)
            {
                saveInDbTasks.Add(SaveInDbTask(list));
            }

            return saveInDbTasks;
        }

        private async Task SaveInDbTask(IEnumerable<ExcelDto> records)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<AppDbContext>();

                var dbRecords = records.Select(p => new ExcelDb() { Id = p.Id }).Distinct().ToList();

                await dbContext.BulkInsertAsync(dbRecords, options =>
                {
                    options.InsertIfNotExists = true;
                    options.BatchSize = 1000;
                });
            }
        }
    }
}