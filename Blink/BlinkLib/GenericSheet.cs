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

using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Collections.Generic;

namespace BlinkLib
{
    class GenericSheet
    {

        const string SHEET_HEADER_NUMBER = "Number";
        const string SHEET_HEADER_FULLNAME = "File Name";
        const string SHEET_HEADER_EXTENSION = "Extension";
        const string SHEET_HEADER_PATH = "Relative Path";
        const string SHEET_HEADER_DESCRIPTION = "Description";

        const int SHEET_INITIAL_ROW = 1;

        const int SHEET_COLUMN_NUMBER = 1;
        const int SHEET_COLUMN_FULLNAME = 2;
        const int SHEET_COLUMN_EXTENSION = 3;
        const int SHEET_COLUMN_DESCRIPTION = 4;
        const int SHEET_COLUMN_PATH = 5;

        public string SheetName { get; set; }
        public ExcelPackage WorkingPackage { get; set; }
        public List<CustomFileInfo> Content { get; set; }
        public long TableNumber { get; set; }

        public void Generate()
        {
            ExcelWorksheet ws = WorkingPackage.Workbook.Worksheets.Add(SheetName);

            int currentRow = SHEET_INITIAL_ROW;

            // Table Headers

            ws.Cells[currentRow, SHEET_COLUMN_NUMBER].Value = SHEET_HEADER_NUMBER;
            ws.Cells[currentRow, SHEET_COLUMN_FULLNAME].Value = SHEET_HEADER_FULLNAME;
            ws.Cells[currentRow, SHEET_COLUMN_EXTENSION].Value = SHEET_HEADER_EXTENSION;
            ws.Cells[currentRow, SHEET_COLUMN_DESCRIPTION].Value = SHEET_HEADER_DESCRIPTION;
            ws.Cells[currentRow, SHEET_COLUMN_PATH].Value = SHEET_HEADER_PATH;

            currentRow++;

            // Table Content

            foreach (CustomFileInfo customFileInfo in Content)
            {
                ws.Cells[currentRow, SHEET_COLUMN_NUMBER].Value = currentRow - SHEET_INITIAL_ROW;
                ws.Cells[currentRow, SHEET_COLUMN_FULLNAME].Value = customFileInfo.FileName;
                ws.Cells[currentRow, SHEET_COLUMN_EXTENSION].Value = customFileInfo.Extension;
                ws.Cells[currentRow, SHEET_COLUMN_PATH].Value = customFileInfo.RelativePath;

                currentRow++;
            }

            // Create a Table

            using (ExcelRange range = ws.Cells[SHEET_INITIAL_ROW, SHEET_COLUMN_NUMBER, currentRow - 1, SHEET_COLUMN_PATH])
            {
                ExcelTableCollection tblCollection = ws.Tables;
                ExcelTable table = tblCollection.Add(range, $"Table{this.TableNumber}");
                table.TableStyle = TableStyles.Dark11;
            }

            // Setting all up

            ws.Cells.AutoFitColumns();

            ws.View.ShowGridLines = false;

            WorkingPackage.Save();
        }
    }
}