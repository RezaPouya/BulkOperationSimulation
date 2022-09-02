using Domain.Shared.DTOs;
using System;
using System.Threading.Tasks;

namespace Operator.Helpers
{
    public static class ExcelUtilityHelper
    {
        public static async Task<ExcelDto[]> GenerateAsync(uint count)
        {
            ExcelDto[] objs = new ExcelDto[count];

            for (int i = 0; i <= count - 1; i++)
            {
                objs[i] = new ExcelDto(Guid.NewGuid());
            }

            return objs;
        }
    }
}