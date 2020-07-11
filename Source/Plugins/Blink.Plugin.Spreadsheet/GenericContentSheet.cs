using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Blink.Plugin.Spreadsheet
{
    public class GenericContentSheet : ContentSheet
    {
        private const string ColumnName_Number = "Number";
        private const string ColumnName_Fullname = "File Name";
        private const string ColumnName_Extension = "Extension";
        private const string ColumnName_Path = "Relative Path";

        private const int ColumnPosition_Number = 1;
        private const int ColumnPosition_Fullname = 2;
        private const int ColumnPosition_Extension = 3;
        private const int ColumnPosition_Path = 4;
        
        protected override void PrintContentRow(ExcelWorksheet excelWorksheet, FileInfo fileInfo)
        {
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Number].Value = CurrentRow - InitialRow;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Fullname].Value = fileInfo.Name;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Extension].Value = fileInfo.Extension;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Path].Value = fileInfo.FullName.Substring(BaseFolder.Length);
        }

        protected override void PrintHeaderRow(ExcelWorksheet excelWorksheet)
        {
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Number].Value = ColumnName_Number;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Fullname].Value = ColumnName_Fullname;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Extension].Value = ColumnName_Extension;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Path].Value = ColumnName_Path;
            excelWorksheet.View.FreezePanes(CurrentRow, ColumnPosition_Fullname);
        }

        protected override void PrintTable(ExcelWorksheet excelWorksheet)
        {
            using (var range = excelWorksheet.Cells[InitialRow, InitialColumn, CurrentRow - 1, ColumnPosition_Path])
            {
                var tblCollection = excelWorksheet.Tables;
                var table = tblCollection.Add(range, $"Table{TableCorrelative}");

                table.TableStyle = TableStyles.Dark11;
            }
        }
    }
}
