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

using Blink.Plugin.CommonConfiguration;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Blink.Plugin.Spreadsheet
{
    public class Main : JsonConfigurationFile<Folder>, IBlink
    {
        private const string LabelAll = "(All)";
        private ContentSheet _contentSheet;
        private HashSet<string> _listOfLabels;
        private Dictionary<string, List<FileInfo>> _map;

        public string FileName { get; private set; }
        public DirectoryInfo WorkingDirectory { get; set; }

        private void Initialize(ContentSheet contentSheet)
        {
            _contentSheet = contentSheet;
        }

        public Main()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Initialize(new GenericContentSheet());

            _contentSheet.ShowGridLines = false;
            _contentSheet.AutoFitColumns = true;
        }

        public Main(ContentSheet contentSheet)
        {
            Initialize(contentSheet);
        }

        public void ExecuteTask()
        {
            try
            {
                LoadConfiguration();
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
            catch (FormatException ex)
            {
                throw new BlinkException(
                    $"Invalid Directory name: \"{ex.Message}\", check your \"{ConfigurationFile}\" file.");
            }
            catch (Exception ex)
            {
                throw new BlinkException(
                    $"There was an error while parsing configuration file: \"{ConfigurationFile}\".", ex);
            }

            _listOfLabels = new HashSet<string>();
            _map = new Dictionary<string, List<FileInfo>>();

            BrowseFiles();

            GenerateListOfLabels();

            GenerateExcelFile();
        }

        public void Init(PluginDetail pluginDetail)
        {
            if (string.IsNullOrWhiteSpace(ConfigurationFile))
                ConfigurationFile = Path.Combine(pluginDetail.PluginDirectory, DefaultConfigurationFile);
        }

        private void GenerateListOfLabels()
        {
            _listOfLabels = new HashSet<string>();

            foreach (var folder in ReadOnlyItemList)
            {
                if (!folder.Browsable)
                    continue;

                var currentPath = Path.Combine(WorkingDirectory.FullName, folder.Name);

                if (!string.IsNullOrWhiteSpace(folder.Label))
                {
                    var currentLabel = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(folder.Label);

                    if (_listOfLabels.Add(currentLabel))
                        _map.Add(currentLabel, new List<FileInfo>());

                    _map[currentLabel].AddRange(_map[LabelAll].FindAll(cfi => cfi.FullName.Contains(currentPath)));

                    continue;
                }

                if (folder.SubFolders == null)
                    continue;

                GenerateListOfLabels(folder.SubFolders, currentPath);
            }
        }

        private void GenerateListOfLabels(List<Folder> subFolders, string parentPath)
        {
            foreach (var folder in subFolders)
            {
                if (!folder.Browsable)
                    continue;

                var currentPath = Path.Combine(parentPath, folder.Name);

                if (!string.IsNullOrWhiteSpace(folder.Label))
                {
                    var currentLabel = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(folder.Label);

                    if (_listOfLabels.Add(currentLabel))
                        _map.Add(currentLabel, new List<FileInfo>());

                    _map[currentLabel].AddRange(_map[LabelAll].FindAll(cfi => cfi.FullName.Contains(currentPath)));

                    continue;
                }

                if (folder.SubFolders == null)
                    continue;

                GenerateListOfLabels(folder.SubFolders, currentPath);
            }
        }

        private void BrowseFiles()
        {
            FileInfo[] files;
            FileInfo[] filtered;

            try
            {
                files = WorkingDirectory.GetFiles("*.*", SearchOption.AllDirectories);

                // Don't show hidden files
                filtered = files.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)).ToArray();

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

            if (!filtered.Any())
                throw new BlinkException($"\"{WorkingDirectory}\" is empty. Did you lost your files?");

            _map.Add(LabelAll, filtered.ToList());
        }

        private void GenerateExcelFile()
        {
            FileName = GenerateTemporaryFileName("xlsx");
            FileInfo newFile = new FileInfo(FileName);

            using (var pck = new ExcelPackage(newFile))
            {

                long tableNumber = 0;

                foreach (var entry in _map)
                {

                    if (!entry.Value.Any())
                        continue;

                    _contentSheet.SheetName = entry.Key;
                    _contentSheet.Content = entry.Value;
                    _contentSheet.BaseFolder = WorkingDirectory.FullName;
                    _contentSheet.TableCorrelative = ++tableNumber;

                    try
                    {
                        _contentSheet.Generate(pck);
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
        }

        private static string GenerateTemporaryFileName(string extension)
        {
            var tempPath = Path.GetTempPath();
            var fileName = $"{Guid.NewGuid()}.{extension}";

            return Path.Combine(tempPath, fileName);
        }
    }
}
