﻿using OfficeOpenXml;
using System.Data;
using System.Reflection;

namespace ExcelHelper
{
    public interface IExcelGeneratorHelper
    {
        Task<string> Generate<T>(IEnumerable<T> data, string fileName);
    }

    public sealed class ExcelGeneratorHelper : IExcelGeneratorHelper
    {
        public async Task<string> Generate<T>(IEnumerable<T> data, string fileName)
        {
            var path = GetFilePath(fileName);

            DataTable table = CreateDataTable(data);
            using (var excelPack = new ExcelPackage(path))
            {
                var ws = excelPack.Workbook.Worksheets.Add("1");
                ws.Cells.LoadFromDataTable(table, true, OfficeOpenXml.Table.TableStyles.Light8);
                await excelPack.SaveAsync();
            }

            return path;
        }

        private static string GetFilePath(string fileName)
        {
            var path = Path.Combine(Environment.CurrentDirectory.ToLower(), fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return path;
        }

        private static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            dataTable.TableName = typeof(T).FullName;
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}