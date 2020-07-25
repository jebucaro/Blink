/*
 * MIT License
 * 
 * Copyright (c) 2020 Jonathan Búcaro
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
using System.IO;

namespace Blink.Plugin.GitLog
{
    class ContentSheet
    {
        private const string HeaderSheet_Name = "Commits";
        private const string DetailSheet_Name = "Detail";

        private const string ColumnName_Commit = "Commit Number";
        private const string ColumnName_Sha = "Sha";
        private const string ColumnName_Message = "Message";
        private const string ColumnName_AuthorName = "Author Name";
        private const string ColumnName_AuthorEmail = "Author Email";
        
        private const string ColumnName_Status = "Status";
        private const string ColumnName_Path = "Relative Path";
        private const string ColumnName_File = "File";
        private const string ColumnName_Extension = "Extension";

        private const int ColumnPosition_Commit = 1;
        private const int ColumnPosition_Sha = 2;
        private const int ColumnPosition_Message = 3;
        private const int ColumnPosition_AuthorName = 4;
        private const int ColumnPosition_AuthorEmail = 5;

        private const int ColumnPosition_Status = 2;
        private const int ColumnPosition_File = 3;
        private const int ColumnPosition_Extension = 4;
        private const int ColumnPosition_Path = 5;

        public bool AutoFitColumns { get; set; }
        public bool ShowGridLines { get; set; }
        
        int CurrentRowHeader;
        int CurrentRowDetail;

        public ContentSheet()
        {
            CurrentRowHeader = 1;
            CurrentRowDetail = 1;
        }

        private void PrintHeaderColumnTitles(ExcelWorksheet excelWorksheet)
        {
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_Commit].Value = ColumnName_Commit;
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_Sha].Value = ColumnName_Sha;
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_Message].Value = ColumnName_Message;
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_AuthorName].Value = ColumnName_AuthorName;
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_AuthorEmail].Value = ColumnName_AuthorEmail;
        }

        private void PrintDetailColumnTitles(ExcelWorksheet excelWorksheet)
        {
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Commit].Value = ColumnName_Commit;
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Status].Value = ColumnName_Status;
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_File].Value = ColumnName_File;
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Extension].Value = ColumnName_Extension;
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Path].Value = ColumnName_Path;
        }

        private void PrintHeader(ExcelWorksheet excelWorksheet, CommitInfo commit)
        {
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_Commit].Value = CurrentRowHeader - 1;
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_Sha].Value = commit.Id;
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_Message].Value = commit.Message;
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_AuthorName].Value = commit.AuthorName;
            excelWorksheet.Cells[CurrentRowHeader, ColumnPosition_AuthorEmail].Value = commit.AuthorEmail;
        }

        private void PrintDetail(ExcelWorksheet excelWorksheet, CommitDetail detail)
        {
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Commit].Value = CurrentRowHeader - 1;
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Status].Value = detail.Status;
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_File].Value = Path.GetFileName(detail.Path);
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Extension].Value = Path.GetExtension(detail.Path);
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Path].Value = Path.GetDirectoryName(detail.Path);
        }

        private void PrintTable(ExcelWorksheet excelWorksheet, int rows, int columns)
        {
            using (var range = excelWorksheet.Cells[1, 1, rows - 1, columns])
            {
                var tblCollection = excelWorksheet.Tables;
                var table = tblCollection.Add(range, $"Table{excelWorksheet.Name}");

                table.TableStyle = TableStyles.Dark11;
            }
        }

        public void Generate(ExcelPackage excelPackage, CustomGitLog log)
        {
            using (var headerSheet = excelPackage.Workbook.Worksheets.Add(HeaderSheet_Name))
            {
                using (var detailSheet = excelPackage.Workbook.Worksheets.Add(DetailSheet_Name))
                {

                    PrintHeaderColumnTitles(headerSheet);
                    PrintDetailColumnTitles(detailSheet);

                    CurrentRowHeader++;
                    CurrentRowDetail++;

                    foreach (var commit in log.Commits)
                    {
                        PrintHeader(headerSheet, commit);

                        foreach (var detail in commit.Detail)
                        {
                            PrintDetail(detailSheet, detail);
                            CurrentRowDetail++;
                        }

                        CurrentRowHeader++;
                    }

                    if (AutoFitColumns)
                    {
                        headerSheet.Cells.AutoFitColumns();
                        detailSheet.Cells.AutoFitColumns();
                    }

                    if (!ShowGridLines)
                    {
                        headerSheet.View.ShowGridLines = false;
                        detailSheet.View.ShowGridLines = false;
                    }

                    PrintTable(headerSheet, CurrentRowHeader, ColumnPosition_AuthorEmail);
                    PrintTable(detailSheet, CurrentRowDetail, ColumnPosition_Path);

                    excelPackage.Save();
                };
            };

            CurrentRowDetail = 1;
            CurrentRowHeader = 1;
        }

    }
}
