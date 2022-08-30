using ExcelHelper.Helpers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ExcelHelper.Tests
{
    public class IExcelGeneratorHelperTest
    {
        [Theory]
        [InlineData(500_000, "excel1.xlsx", true)]
        public async Task Should_be_able_to_create_a_list_Of_500k_excel_objs(uint size, string fileName, bool fileIsExist)
        {
            // arrange
            var objs = await ExcelUtilityHelper.GenerateAsync(size);
            IExcelGeneratorHelper generator = new ExcelGeneratorHelper();

            // act
            var filePath = await generator.Generate(objs , fileName);

            // assert

            Assert.Equal(fileIsExist, File.Exists(filePath));
        }
    }
}