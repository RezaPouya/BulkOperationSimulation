using ExcelHelper.Models;

namespace ExcelHelper.Helpers
{
    public static class ExcelUtilityHelper
    {
        public static async Task<ExcelObj[]> GenerateAsync(uint count)
        {
            ExcelObj[] objs = new ExcelObj[count];

            for (int i = 0; i <= count - 1; i++)
            {
                objs[i] = new ExcelObj(Guid.NewGuid());
            }

            return objs;
        }
    }
}