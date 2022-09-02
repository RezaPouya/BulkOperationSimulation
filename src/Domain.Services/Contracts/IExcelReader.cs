using Domain.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services.Contracts
{
    public interface IExcelReader<T> where T : ExcelDto, new()
    {
        Task<IEnumerable<T>> ReadFileToObjAsync(string filePath, int? excelRowCount);
    }
}