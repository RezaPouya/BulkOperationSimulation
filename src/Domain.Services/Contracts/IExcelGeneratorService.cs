using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services.Contracts
{
    public interface IExcelGeneratorService
    {
        Task<string> Generate<T>(IEnumerable<T> data, string fileName);
    }
}