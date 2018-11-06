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

using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BlinkLib
{
    public class GenerateSpreadsheetCommand : BlinkCommand
    {

        const string CONFIGURATION_FILE = "branch.settings.json";
        const string LABEL_ALL = "(All)";

        List<Branch> _folderStructure;

        HashSet<string> _listOfLabels;
        Dictionary<string, List<CustomFileInfo>> _map = new Dictionary<string, List<CustomFileInfo>>();

        public GenerateSpreadsheetCommand(WorkingDirectory workingDirectory) : base(workingDirectory){ }

        public string FileName { get; private set; }

        protected override void ExecuteTask()
        {
            _map.Add(LABEL_ALL, new List<CustomFileInfo>());

            BrowseFiles();
            GenerateListOfLabels();
            GenerateExcelFile();
        }

        protected override void LoadConfiguration()
        {
            try
            {
                using (StreamReader r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIGURATION_FILE)))
                {
                    string json = r.ReadToEnd();
                    _folderStructure = JsonConvert.DeserializeObject<List<Branch>>(json);
                }
            }
            catch (FileNotFoundException)
            {
                throw new BlinkException($"Unable to find configuration file: \"{CONFIGURATION_FILE}\".");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException($"Unable to access Blink application directory \"{AppDomain.CurrentDomain.BaseDirectory}\".");
            }
            catch (JsonException)
            {
                throw new BlinkException($"There was an error while parsing configuration file: \"{CONFIGURATION_FILE}\".");
            }
            catch (FormatException ex)
            {
                throw new BlinkException($"Invalid Directory name: \"{ex.Message}\", check your \"{CONFIGURATION_FILE}\" file.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GenerateListOfLabels()
        {
            _listOfLabels = new HashSet<string>();

            foreach (Branch tree in _folderStructure)
            {
                if (!tree.Browsable)
                    continue;

                string currentPath = Path.Combine(this.workingDirectory.Path, tree.Name);

                if (!String.IsNullOrWhiteSpace(tree.Label))
                {
                    string currentLabel = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(tree.Label);

                    if (_listOfLabels.Add(currentLabel))
                        _map.Add(currentLabel, new List<CustomFileInfo>());

                    _map[currentLabel].AddRange(_map[LABEL_ALL].FindAll(cfi => cfi.FullPath.Contains(currentPath)));

                    continue;
                }

                if (tree.Branches == null)
                    continue;

                GenerateListOfLabels(tree.Branches, currentPath);
            }
        }

        private void GenerateListOfLabels(List<Branch> tree, string parentPath)
        {
            foreach (Branch branch in tree)
            {
                if (!branch.Browsable)
                    continue;

                string currentPath = Path.Combine(parentPath, branch.Name);

                if (!String.IsNullOrWhiteSpace(branch.Label))
                {
                    string currentLabel = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(branch.Label);

                    if (_listOfLabels.Add(currentLabel))
                        _map.Add(currentLabel, new List<CustomFileInfo>());

                    _map[currentLabel].AddRange(_map[LABEL_ALL].FindAll(cfi => cfi.FullPath.Contains(currentPath)));

                    continue;
                }

                if (branch.Branches == null)
                    continue;

                GenerateListOfLabels(branch.Branches, currentPath);
            }
        }

        private CustomFileInfo GetCustomFileInfo(string filePath, string parentPath)
        {
            CustomFileInfo result = new CustomFileInfo();

            try
            {
                result.FileName = Path.GetFileName(filePath);
            }
            catch (Exception)
            {
                result.FileName = string.Empty;
            }

            try
            {
                result.FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            }
            catch (Exception)
            {
                result.FileNameWithoutExtension = string.Empty;
            }

            try
            {
                result.Extension = Path.GetExtension(filePath).Substring(1);
            }
            catch (Exception)
            {
                result.Extension = string.Empty;
            }

            try
            {
                result.RelativePath = Path.GetDirectoryName(filePath).Substring(parentPath.Length);
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
            string[] _files;

            try
            {
                _files = Directory.GetFiles(this.workingDirectory.Path, "*.*", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"There is a Directory or File inside WorkingDirectory \"{this.workingDirectory.Path}\" that is not currently accesible.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException($"There is a Directory or File inside WorkingDirectory \"{this.workingDirectory.Path}\" that is not currently accesible.");
            }
            catch (PathTooLongException)
            {
                throw new BlinkException($"There is a Directory or File inside WorkingDirectory \"{this.workingDirectory.Path}\" that is name is too long.");
            }
            catch (IOException)
            {
                throw new BlinkException($"There is a Directory or File inside WorkingDirectory \"{this.workingDirectory.Path}\" that is not currently accesible.");
            }
            catch (Exception)
            {
                throw;
            }

            if (_files.Count() == 0)
                throw new BlinkException($"WorkingDirectory \"{this.workingDirectory.Path}\" is empty. Did you lost your files?");

            foreach (string currentFile in _files)
                _map[LABEL_ALL].Add(GetCustomFileInfo(currentFile, this.workingDirectory.Path));
        }

        private void GenerateExcelFile()
        {
            FileInfo newFile;
            ExcelPackage pck;

            try
            {
                this.FileName = GenerateTemporaryFileName("xlsx");
                newFile = new FileInfo(this.FileName);
                pck = new ExcelPackage(newFile);
            }
            catch (Exception)
            {
                throw;
            }

            long tableNumber = 0;

            foreach (KeyValuePair<string, List<CustomFileInfo>> entry in _map)
            {

                if (entry.Value.Count() <= 0)
                    continue;

                try
                {
                    GenericSheet sheet = new GenericSheet
                    {
                        WorkingPackage = pck,
                        SheetName = entry.Key,
                        Content = entry.Value,
                        TableNumber = ++tableNumber
                    };

                    sheet.Generate();
                }
                catch (Exception ex)
                {
                    throw new BlinkException($"There was an error while genereting the Spreadsheet file: {ex.Message}");
                }
            }

            Process.Start(newFile.FullName);
        }

        string GenerateTemporaryFileName(string extension)
        {
            string tempPath = Path.GetTempPath();
            string fileName = $"{Guid.NewGuid().ToString()}.{extension}";

            return Path.Combine(tempPath, fileName);
        }
    }
}
