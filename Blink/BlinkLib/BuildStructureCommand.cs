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
using System;
using System.Collections.Generic;
using System.IO;

namespace BlinkLib
{
    public class BuildStructureCommand : BlinkCommand
    {
        const string CONFIGURATION_FILE = "branch.settings.json";

        List<Branch> _folderStructure;

        public BuildStructureCommand(WorkingDirectory workingDirectory) : base(workingDirectory){ }

        protected override void ExecuteTask()
        {
            CreateFolder();
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

        private void CreateFolder()
        {
            string currentPath = String.Empty;

            try
            {
                foreach (Branch currentBranch in _folderStructure)
                {
                    currentPath = Path.Combine(this.workingDirectory.Path, currentBranch.Name);

                    Directory.CreateDirectory(currentPath);

                    if (currentBranch.Branches != null)
                        CreateFolder(currentBranch, currentBranch.Name);
                }
            }
            catch (BlinkException)
            {
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"Directory \"{currentPath}\" is not currently accesible.");
            }
            catch (PathTooLongException)
            {
                throw new BlinkException($"The path to the Directory \"{currentPath}\" is too long");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException($"Directory not found \"{currentPath}\".");
            }
            catch (NotSupportedException)
            {
                throw new BlinkException($"The path to the Directory \"{currentPath}\" contains a colon character (:) that is not a part of a drive label.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CreateFolder(Branch node, string rootPath)
        {
            string currentPath = String.Empty;

            try
            {
                foreach (Branch currentBranch in node.Branches)
                {
                    currentPath = Path.Combine(this.workingDirectory.Path, rootPath, currentBranch.Name);

                    Directory.CreateDirectory(currentPath);

                    if (currentBranch.Branches != null)
                        CreateFolder(currentBranch, Path.Combine(rootPath, currentBranch.Name));
                }
            }
            catch (BlinkException)
            {
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"Directory \"{currentPath}\" is not currently accesible.");
            }
            catch (PathTooLongException)
            {
                throw new BlinkException($"The path to the Directory \"{currentPath}\" is too long");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException($"Directory not found \"{currentPath}\".");
            }
            catch (NotSupportedException)
            {
                throw new BlinkException($"The path to the Directory \"{currentPath}\" contains a colon character (:) that is not a part of a drive label.");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
