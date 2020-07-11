using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Blink.Plugin.Spreadsheet
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

        public ContentSheet()
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
