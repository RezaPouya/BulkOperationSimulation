using ExcelHelper.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExcelHelper.Tests
{
    public class GenerateExcelHelperTest
    {
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
    }
}