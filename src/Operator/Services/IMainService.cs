using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Operator.DTOs;
using Operator.Extensions;
using Operator.Helpers;
using Operator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Operator.Services
{
    public interface IMainService
    {
        Task DoMainOperation();
    }

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
            await SaveInDb(redisObjects.ToList());
            Console.WriteLine("---------------------------------------------------------------------------");

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
            await _distributedCache.SetAsync(fileName.GetRedisKey(), excelObjects.SerializeToBinary(), default);
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
            return objects;
        }

        private async Task SaveInDb(List<ExcelDto> records, int batchSize = 1000)
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

            ICollection<Task> saveInDbTasks = CreateBatchSaveInDbTasks(records, batchSize);

            await Task.WhenAll(saveInDbTasks);

            sw.Stop();
            Console.WriteLine($"The records are saved in db in '{sw.ElapsedMilliseconds}' milli seconds");
        }

        private ICollection<Task> CreateBatchSaveInDbTasks(List<ExcelDto> records, int batchSize)
        {
            ICollection<Task> saveInDbTasks = new Collection<Task>();

            var count = (records.Count() / batchSize);

            int minIndex = 0;

            for (int i = 0; i < count; i++)
            {
                if (i == count - 1)
                {
                    var tsk = SaveInDbTask(records.GetElements<ExcelDto>(minIndex, records.Count()));
                    saveInDbTasks.Add(tsk);
                }
                else
                {
                    var maxIndex = minIndex + batchSize;
                    var tsk = SaveInDbTask(records.GetElements<ExcelDto>(minIndex, maxIndex));
                    saveInDbTasks.Add(tsk);
                }

                minIndex = i + batchSize + 1;
            }

            return saveInDbTasks;
        }

        private async Task SaveInDbTask(IEnumerable<ExcelDto> records)
        {
            var dbContext = _serviceProvider.GetService<AppDbContext>();
            dbContext.AddRange(records);
            await dbContext.SaveChangesAsync();
        }
    }
}