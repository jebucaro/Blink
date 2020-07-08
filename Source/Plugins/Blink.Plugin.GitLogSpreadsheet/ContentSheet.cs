using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Blink.Plugin.GitLogSpreadsheet
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
        
        private const string ColumnName_Correlative = "Correlative";
        private const string ColumnName_Status = "Status";
        private const string ColumnName_Path = "Path";

        private const int ColumnPosition_Commit = 1;
        private const int ColumnPosition_Sha = 2;
        private const int ColumnPosition_Message = 3;
        private const int ColumnPosition_AuthorName = 4;
        private const int ColumnPosition_AuthorEmail = 5;

        private const int ColumnPosition_Correlative = 2;
        private const int ColumnPosition_Status = 3;
        private const int ColumnPosition_Path = 4;

        public bool AutoFitColumns { get; set; }
        public bool ShowGridLines { get; set; }
        public int InitialRow { get; set; }
        public int InitialColumn { get; set; }
        
        int CurrentRowHeader;
        int CurrentRowDetail;
        int CurrentCorrelative;

        public ContentSheet()
        {
            InitialRow = 1;
            InitialColumn = 1;
            CurrentCorrelative = 1;
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
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Correlative].Value = ColumnName_Correlative;
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Status].Value = ColumnName_Status;
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
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Correlative].Value = CurrentCorrelative;
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Status].Value = detail.Status;
            excelWorksheet.Cells[CurrentRowDetail, ColumnPosition_Path].Value = detail.Path;
        }

        private void PrintTable(ExcelWorksheet excelWorksheet, int width, int height)
        {
            using (var range = excelWorksheet.Cells[1, 1, width, height - 1])
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

                        CurrentCorrelative = 1;

                        foreach (var detail in commit.Detail)
                        {
                            PrintDetail(detailSheet, detail);
                            CurrentRowDetail++;
                            CurrentCorrelative++;
                        }

                        CurrentRowHeader++;
                    }

                    detailSheet.Cells.AutoFitColumns();
                    detailSheet.View.ShowGridLines = false;

                    headerSheet.Cells.AutoFitColumns();
                    headerSheet.View.ShowGridLines = false;

                    PrintTable(headerSheet, CurrentRowHeader, ColumnPosition_AuthorEmail);
                    PrintTable(detailSheet, CurrentRowDetail, ColumnPosition_Path);

                    excelPackage.Save();
                };
            };

            

        }

    }
}
