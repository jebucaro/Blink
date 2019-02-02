/*
 * MIT License
 * 
 * Copyright (c) 2018 Jonathan Búcaro
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace BlinkUtils
{
    internal class GenericContentSheet : ContentSheet
    {
        private const string ColumnName_Number = "Number";
        private const string ColumnName_Fullname = "File Name";
        private const string ColumnName_Extension = "Extension";
        private const string ColumnName_Path = "Relative Path";
        private const string ColumnName_Description = "Description";

        private const int ColumnPosition_Number = 1;
        private const int ColumnPosition_Fullname = 2;
        private const int ColumnPosition_Extension = 3;
        private const int ColumnPosition_Description = 4;
        private const int ColumnPosition_Path = 5;

        protected override void PrintHeaderRow(ExcelWorksheet excelWorksheet)
        {
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Number].Value = ColumnName_Number;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Fullname].Value = ColumnName_Fullname;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Extension].Value = ColumnName_Extension;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Description].Value = ColumnName_Description;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Path].Value = ColumnName_Path;
        }

        protected override void PrintContentRow(ExcelWorksheet excelWorksheet, FileInfo fileInfo)
        {
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Number].Value = CurrentRow - InitialRow;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Fullname].Value = fileInfo.Name;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Extension].Value = fileInfo.Extension;
            excelWorksheet.Cells[CurrentRow, ColumnPosition_Path].Value = fileInfo.FullName.Substring(BaseFolder.Length);
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