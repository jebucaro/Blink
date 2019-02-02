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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace BlinkUtils
{
    public abstract class ContentSheet
    {


        public bool AutoFitColumns { get; set; }
        public string SheetName { get; set; }
        public bool ShowGridLines { get; set; }
        public string BaseFolder { get; set; }
        public int InitialRow { get; set; }
        public int InitialColumn { get; set; }

        protected int CurrentRow;

        public List<FileInfo> Content { get; set; }
        public long TableCorrelative { get; set; }

        protected ContentSheet()
        {
            TableCorrelative = 1;
            InitialRow = 1;
            InitialColumn = 1;
            CurrentRow = 1;
        }
        
        protected abstract void PrintHeaderRow(ExcelWorksheet excelWorksheet);
        protected abstract void PrintContentRow(ExcelWorksheet excelWorksheet, FileInfo fileInfo);
        protected abstract void PrintTable(ExcelWorksheet excelWorksheet);

        public void Generate(ExcelPackage excelPackage)
        {
            var ws = excelPackage.Workbook.Worksheets.Add(SheetName);

            PrintHeaderRow(ws);

            CurrentRow++;

            foreach (var customFileInfo in Content)
            {
                PrintContentRow(ws, customFileInfo);
                CurrentRow++;
            }

            PrintTable(ws);
                
            if (AutoFitColumns)
                ws.Cells.AutoFitColumns();

            ws.View.ShowGridLines = ShowGridLines;

            excelPackage.Save();

            CurrentRow = InitialRow;
        }

    }
}
