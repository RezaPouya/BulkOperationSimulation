using Domain.Services.Contracts;
using Domain.Shared.DTOs;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services.Implementations
{
    public class ExcelReader<T> : IExcelReader<T> where T : ExcelDto, new()
    {
        public async Task<IEnumerable<T>> ReadFileToObjAsync(string filePath, int? excelRowCount)
        {
            List<T> result = excelRowCount != null ? new List<T>(excelRowCount.Value) : new List<T>();

            var fi = new FileInfo(filePath);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(fi))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    result.Add(new T()
                    {
                        Id = new Guid(worksheet.Cells[row, 1].Value.ToString())
                    });
                }

                await package.SaveAsync();
            }

            return result;
        }
    }
}