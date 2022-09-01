using Operator.DTOs;
using Operator.Helpers;
using System;
using System.Diagnostics;
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
        private readonly IExcelReader<ExcelDto> _excelReader;
        private ExcelDto[] _excelObjects ; 
        public MainService(IExcelGeneratorService excelGenerator,
            IExcelReader<ExcelDto> excelReader)
        {
            _excelGenerator = excelGenerator;
            _excelReader = excelReader;
        }

        public async Task DoMainOperation()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Operation starts");
            Console.WriteLine("Step 1 : ");
            await GenerateExcellFile();
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("Step 2 : ");
            await ReadFromExcel();
            Console.WriteLine("---------------------------------------------------------------------------");
            //Console.WriteLine("Step 3 : ");
            //await SaveInRedis();
            //Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("Operation end");
            sw.Stop();
            Console.WriteLine($"operation completed in '{sw.ElapsedMilliseconds}' milli seconds");
        }

        private async Task GenerateExcellFile()
        {
            Console.WriteLine($"Generate excell file with name of 'excel.xlsx' with 500_000 rows...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var objs = await ExcelUtilityHelper.GenerateAsync(500_000);
            await _excelGenerator.Generate(objs, "excel.xlsx");
            sw.Stop();
            Console.WriteLine($"excel file generated in '{sw.ElapsedMilliseconds}' milli seconds");
        }

        private async Task ReadFromExcel()
        {
            Console.WriteLine($"read excel file from file ...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var objs = await ExcelUtilityHelper.GenerateAsync(500_000);
            await _excelGenerator.Generate(objs, "excel.xlsx");
            sw.Stop();
            Console.WriteLine($"excel file read in '{sw.ElapsedMilliseconds}' milli seconds");
        }

        private async Task SaveInRedis()
        {
            Console.WriteLine($"read excel file from file and save it with key of 'excel.xlsx' in redis ...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var objs = await ExcelUtilityHelper.GenerateAsync(500_000);
            await _excelGenerator.Generate(objs, "excel.xlsx");
            sw.Stop();
            Console.WriteLine($"excel file generated in '{sw.ElapsedMilliseconds}' milli seconds");
        }
    }
}