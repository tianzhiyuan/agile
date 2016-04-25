using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Utils
{
    public class ExcelUtil
    {
//        public static void Export(Stream output, IEnumerable<IDictionary<string, object>> data, IEnumerable<KeyValuePair<string, string>> columnMap = null, string sheetName = null, bool use2007 = false)
//        {
//            if (data == null || !data.Any())
//            {
//                throw new ArgumentNullException("data");
//            }
//            if (columnMap == null)
//            {
//                columnMap = data.FirstOrDefault().Keys.ToDictionary(o => o, o => o);
//            }
//            if (sheetName == null)
//            {
//                sheetName = "sheet1";
//            }
//            IWorkbook workbook = use2007 ? (IWorkbook)new XSSFWorkbook() : new HSSFWorkbook();
//            ISheet sheet = workbook.CreateSheet(sheetName);
//
//            IRow firstRow = sheet.CreateRow(0);
//            int columnIndex = 0;
//            foreach (var entry in columnMap)
//            {
//                ICell cell = firstRow.CreateCell(columnIndex);
//                cell.SetCellValue(entry.Value);
//                columnIndex++;
//            }
//            int rowIndex = 1;
//
//            foreach (var item in data)
//            {
//                IRow row = sheet.CreateRow(rowIndex);
//                columnIndex = 0;
//                foreach (var entry in columnMap)
//                {
//                    ICell cell = row.CreateCell(columnIndex);
//                    object value = item[entry.Key];
//                    cell.SetCellValue(value == null ? string.Empty : value.ToString());
//                    columnIndex++;
//                }
//                rowIndex++;
//            }
//
//            workbook.Write(output);
//        }
    }
}
