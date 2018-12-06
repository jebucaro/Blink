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

using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace BlinkLib
{
    internal class GenericSheet
    {
        private const string SheetHeaderNumber = "Number";
        private const string SheetHeaderFullname = "File Name";
        private const string SheetHeaderExtension = "Extension";
        private const string SheetHeaderPath = "Relative Path";
        private const string SheetHeaderDescription = "Description";

        private const int SheetInitialRow = 1;

        private const int SheetColumnNumber = 1;
        private const int SheetColumnFullname = 2;
        private const int SheetColumnExtension = 3;
        private const int SheetColumnDescription = 4;
        private const int SheetColumnPath = 5;

        public string SheetName { get; set; }
        public ExcelPackage WorkingPackage { get; set; }
        public List<CustomFileInfo> Content { get; set; }
        public long TableNumber { get; set; }

        public void Generate()
        {
            var ws = WorkingPackage.Workbook.Worksheets.Add(SheetName);

            var currentRow = SheetInitialRow;

            // Table Headers

            ws.Cells[currentRow, SheetColumnNumber].Value = SheetHeaderNumber;
            ws.Cells[currentRow, SheetColumnFullname].Value = SheetHeaderFullname;
            ws.Cells[currentRow, SheetColumnExtension].Value = SheetHeaderExtension;
            ws.Cells[currentRow, SheetColumnDescription].Value = SheetHeaderDescription;
            ws.Cells[currentRow, SheetColumnPath].Value = SheetHeaderPath;

            currentRow++;

            // Table Content

            foreach (var customFileInfo in Content)
            {
                ws.Cells[currentRow, SheetColumnNumber].Value = currentRow - SheetInitialRow;
                ws.Cells[currentRow, SheetColumnFullname].Value = customFileInfo.FileName;
                ws.Cells[currentRow, SheetColumnExtension].Value = customFileInfo.Extension;
                ws.Cells[currentRow, SheetColumnPath].Value = customFileInfo.RelativePath;

                currentRow++;
            }

            // Create a Table

            using (var range = ws.Cells[SheetInitialRow, SheetColumnNumber, currentRow - 1, SheetColumnPath])
            {
                var tblCollection = ws.Tables;
                var table = tblCollection.Add(range, $"Table{TableNumber}");
                table.TableStyle = TableStyles.Dark11;
            }

            // Setting all up

            ws.Cells.AutoFitColumns();

            ws.View.ShowGridLines = false;

            WorkingPackage.Save();
        }
    }
}