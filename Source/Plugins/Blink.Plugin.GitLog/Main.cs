using LibGit2Sharp;
using OfficeOpenXml;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Blink.Plugin.GitLog
{
    public class Main : IBlink
    {
        public string FileName { get; private set; }
        public DirectoryInfo WorkingDirectory { get; set; }

        private ContentSheet _contentSheet;

        public Main()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private CustomGitLog GetLogFromWorkingDirectory(string path)
        {
            var result = new CustomGitLog();

            try
            {
                using (var repository = new Repository(path))
                {
                    result.Path = path;
                    result.BranchName = repository.Head.FriendlyName;

                    foreach (Commit commit in repository.Commits.Take(25))
                    {

                        CommitInfo commitInfo = new CommitInfo()
                        {
                            Id = commit.Id.ToString(),
                            Message = commit.Message,
                            AuthorName = commit.Author.Name,
                            AuthorEmail = commit.Author.Email
                        };

                        foreach (Commit parent in commit.Parents)
                        {
                            foreach (TreeEntryChanges change in repository.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree))
                            {
                                CommitDetail commitDetail = new CommitDetail()
                                {
                                    Status = change.Status.ToString(),
                                    Path = change.Path
                                };

                                commitInfo.Detail.Add(commitDetail);
                            }
                        }

                        result.Commits.Add(commitInfo);
                    }
                };
            }
            catch (ArgumentNullException ex)
            {
                throw new BlinkException("Working directory is not a valid git repository", ex);
            }
            catch (RepositoryNotFoundException ex)
            {
                throw new BlinkException($"Working directory is not a valid git repository", ex);
            }

            return result;
        }

        private void GenerateExcelFile(CustomGitLog customGitLog)
        {
            
            FileName = GenerateTemporaryFileName("xlsx");
            FileInfo newFile = new FileInfo(FileName);

            using (var pck = new ExcelPackage(newFile))
            {
                try
                {
                    _contentSheet.ShowGridLines = false;
                    _contentSheet.AutoFitColumns = true;
                    _contentSheet.Generate(pck, customGitLog);
                }
                catch (Exception ex)
                {
                    throw new BlinkException($"There was an error while generating the Spreadsheet file: {ex.Message}");
                }

                try
                {
                    Process.Start(newFile.FullName);
                }
                catch (Exception ex)
                {
                    throw new BlinkException($"There was an error while opening the Spreadsheet file: {ex.Message} with current associated program.");
                }
            }
        }

        /// <summary>
        /// Generates a random file name inside TempPath
        /// </summary>
        /// <param name="extension">Extension for temporary file</param>
        /// <returns>Full path to temporary file</returns>
        private static string GenerateTemporaryFileName(string extension = "")
        {
            var fileName = string.Concat(Path.GetRandomFileName().Replace(".", ""),
                (!string.IsNullOrEmpty(extension)) ? (extension.StartsWith(".") ? extension : string.Concat(".", extension)) : "");

            return Path.Combine(Path.GetTempPath(), fileName);
        }

        public void ExecuteTask()
        {
            var customGitLog = GetLogFromWorkingDirectory(WorkingDirectory.FullName);

            GenerateExcelFile(customGitLog);
        }

        public void Init(PluginDetail pluginDetail)
        {
            _contentSheet = new ContentSheet();
        }
    }
}
