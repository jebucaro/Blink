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
using Newtonsoft.Json;
using System.Globalization;
using OfficeOpenXml;
using System.Diagnostics;

namespace BlinkLib
{
    public class GenerateSpreadsheet : Blink
    {
        private const string LabelAll = "(All)";

        private List<Branch> _folderStructure;
        private HashSet<string> _listOfLabels;
        private Dictionary<string, List<FileInfo>> _map = new Dictionary<string, List<FileInfo>>();

        public string FileName { get; private set; }

        public GenerateSpreadsheet(DirectoryInfo directoryInfo):base(directoryInfo) { }

        protected override void LoadConfiguration()
        {
            try
            {
                using (var r = new StreamReader(ConfigurationFile))
                {
                    var json = r.ReadToEnd();
                    _folderStructure = JsonConvert.DeserializeObject<List<Branch>>(json);
                }
            }
            catch (FileNotFoundException)
            {
                throw new BlinkException($"Unable to find configuration file: \"{ConfigurationFile}\".");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException(
                    $"Unable to access Blink application directory \"{AppDomain.CurrentDomain.BaseDirectory}\".");
            }
            catch (JsonException)
            {
                throw new BlinkException(
                    $"There was an error while parsing configuration file: \"{ConfigurationFile}\".");
            }
            catch (FormatException ex)
            {
                throw new BlinkException(
                    $"Invalid Directory name: \"{ex.Message}\", check your \"{ConfigurationFile}\" file.");
            }
        }

        protected override void ExecuteTask()
        {
            BrowseFiles();

            GenerateListOfLabels();

            GenerateExcelFile();
        }

        private void GenerateListOfLabels()
        {
            _listOfLabels = new HashSet<string>();

            foreach (var tree in _folderStructure)
            {
                if (!tree.Browsable)
                    continue;

                var currentPath = Path.Combine(WorkingDirectory.FullName, tree.Name);

                if (!string.IsNullOrWhiteSpace(tree.Label))
                {
                    var currentLabel = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(tree.Label);

                    if (_listOfLabels.Add(currentLabel))
                        _map.Add(currentLabel, new List<FileInfo>());

                    _map[currentLabel].AddRange(_map[LabelAll].FindAll(cfi => cfi.FullName.Contains(currentPath)));

                    continue;
                }

                if (tree.Branches == null)
                    continue;

                GenerateListOfLabels(tree.Branches, currentPath);
            }
        }

        private void GenerateListOfLabels(List<Branch> tree, string parentPath)
        {
            foreach (var branch in tree)
            {
                if (!branch.Browsable)
                    continue;

                var currentPath = Path.Combine(parentPath, branch.Name);

                if (!string.IsNullOrWhiteSpace(branch.Label))
                {
                    var currentLabel = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(branch.Label);

                    if (_listOfLabels.Add(currentLabel))
                        _map.Add(currentLabel, new List<FileInfo>());

                    _map[currentLabel].AddRange(_map[LabelAll].FindAll(cfi => cfi.FullName.Contains(currentPath)));

                    continue;
                }

                if (branch.Branches == null)
                    continue;

                GenerateListOfLabels(branch.Branches, currentPath);
            }
        }

        private static CustomFileInfo GetCustomFileInfo(string filePath, string parentPath)
        {
            var result = new CustomFileInfo();

            try
            {
                result.FileName = System.IO.Path.GetFileName(filePath);
            }
            catch (Exception)
            {
                result.FileName = string.Empty;
            }

            try
            {
                result.FileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
            catch (Exception)
            {
                result.FileNameWithoutExtension = string.Empty;
            }

            try
            {
                result.Extension = System.IO.Path.GetExtension(filePath)?.Substring(1);
            }
            catch (Exception)
            {
                result.Extension = string.Empty;
            }

            try
            {
                result.RelativePath = System.IO.Path.GetDirectoryName(filePath)?.Substring(parentPath.Length);
            }
            catch (Exception)
            {
                result.RelativePath = string.Empty;
            }

            result.FullPath = filePath;

            return result;
        }

        private void BrowseFiles()
        {
            string[] files;
            FileInfo[] files2;
            try
            {
                files = Directory.GetFiles(WorkingDirectory.FullName, "*.*", SearchOption.AllDirectories);
                files2 = WorkingDirectory.GetFiles("*.*", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"There is a Directory or File inside \"{WorkingDirectory}\" that is not currently accessible.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException($"There is a Directory or File inside \"{WorkingDirectory}\" that is not currently accessible.");
            }
            catch (PathTooLongException)
            {
                throw new BlinkException($"There is a Directory or File inside \"{WorkingDirectory}\" that is name is too long.");
            }
            catch (IOException)
            {
                throw new BlinkException($"There is a Directory or File inside \"{WorkingDirectory}\" that is not currently accessible.");
            }

            if (!files.Any())
                throw new BlinkException($"\"{WorkingDirectory}\" is empty. Did you lost your files?");

            _map.Add(LabelAll, files2.ToList<FileInfo>());
        }

        private void GenerateExcelFile()
        {
            FileName = GenerateTemporaryFileName("xlsx");
            var newFile = new FileInfo(FileName);
            var pck = new ExcelPackage(newFile);

            long tableNumber = 0;

            foreach (var entry in _map)
            {

                if (!entry.Value.Any())
                    continue;

                var sheet = new GenericSheet
                {
                    WorkingPackage = pck,
                    SheetName = entry.Key,
                    Content = entry.Value,
                    TableNumber = ++tableNumber
                };

                try
                {
                    sheet.Generate();
                }
                catch (Exception ex)
                {
                    throw new BlinkException($"There was an error while generating the Spreadsheet file: {ex.Message}");
                }
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

        private static string GenerateTemporaryFileName(string extension)
        {
            var tempPath = System.IO.Path.GetTempPath();
            var fileName = $"{Guid.NewGuid().ToString()}.{extension}";

            return System.IO.Path.Combine(tempPath, fileName);
        }
    }
}
