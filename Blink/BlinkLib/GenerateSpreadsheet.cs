﻿/*
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
using System.Globalization;
using OfficeOpenXml;
using System.Diagnostics;

namespace BlinkLib
{
    public class GenerateSpreadsheet : Blink
    {
        private const string LabelAll = "(All)";

        private HashSet<string> _listOfLabels;
        private Dictionary<string, List<FileInfo>> _map = new Dictionary<string, List<FileInfo>>();

        /// <summary>
        /// Gets the spreadsheet name generated by GenerateSpreadsheet
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Creates a new instance of GenerateSpreadsheet
        /// </summary>
        public GenerateSpreadsheet() { }

        /// <summary>
        /// Creates a new spreadsheet file based on WorkingDirectory and configuration file
        /// </summary>
        protected override void ExecuteTask()
        {
            BrowseFiles();

            GenerateListOfLabels();

            GenerateExcelFile();
        }

        /// <summary>
        /// Generates a list of labels from the configuration file
        /// </summary>
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

        /// <summary>
        /// Generates a list of labels from the configuration file
        /// </summary>
        /// <param name="tree">Current node</param>
        /// <param name="parentPath">Parent Directory path</param>
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

        /// <summary>
        /// Browse all files inside WorkingDirectory
        /// </summary>
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

        /// <summary>
        /// Creates a new spreadsheet file based on WorkingDirectory and configuration file
        /// </summary>
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
                    WorkingDirectory = this.WorkingDirectory.FullName,
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

        /// <summary>
        /// Generates a randon file name inside TempPath
        /// </summary>
        /// <param name="extension">Extension for temporary file</param>
        /// <returns>Full path to temporary file</returns>
        private static string GenerateTemporaryFileName(string extension)
        {
            var tempPath = Path.GetTempPath();
            var fileName = $"{Guid.NewGuid().ToString()}.{extension}";

            return Path.Combine(tempPath, fileName);
        }
    }
}
