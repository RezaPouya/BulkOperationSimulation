using Operator.DTOs;
using Operator.Helpers;
using Operator.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OperatorTest.Tests
{
    public class IExcelGeneratorHelperTest
    {
        [Theory]
        [InlineData(500_000, "excel1.xlsx", true)]
        public async Task Should_be_able_to_create_a_list_Of_500k_excel_objs(uint size, string fileName, bool fileIsExist)
        {
            // arrange
            var objs = await ExcelUtilityHelper.GenerateAsync(size);

            IExcelGeneratorService generator = new ExcelGeneratorService();

            // act
            var filePath = await generator.Generate(objs, fileName);

            // assert

            Assert.Equal(fileIsExist, File.Exists(filePath));

        }

        [Theory]
        [InlineData(500_000, 500_000)]
        public async Task Should_be_able_to_create_a_list_Of_obj_excel_objs(uint size, int generated)
        {
            var objs = await ExcelUtilityHelper.GenerateAsync(size);

            Assert.Equal(generated, objs.Length);
        }

        [Theory]
        [InlineData(500_000)]
        public async Task Should_be_able_to_create_a_list_Of_obj_without_duplicate_value_excel_objs2(uint size)
        {
            var objs = await ExcelUtilityHelper.GenerateAsync(size);
            var objsDistinct = objs.Distinct().ToList();
            Assert.Equal(objs.Length, objsDistinct.Count());
        }

        [Fact]
        public async Task Should_be_able_to_read_excel_file_and_cast_it_to_object()
        {
            uint size = 5_000;
            string fileName = $"excel1.xlsx";

            var objs = await ExcelUtilityHelper.GenerateAsync(size);

            IExcelGeneratorService generator = new ExcelGeneratorService();

            var filePath = await generator.Generate(objs, fileName);

            var path = Path.Combine(Environment.CurrentDirectory.ToLower(), fileName);

            var reader = new ExcelReader<ExcelDto>();
            
            // Act
            var read_objs = await reader.ReadFileToObjAsync(path , (int)size);


            // Assert 
            Assert.NotNull(read_objs);

            Assert.Equal(size.ToString(), read_objs.Count().ToString());
        }
    }
}