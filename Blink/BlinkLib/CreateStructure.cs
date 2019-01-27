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
using Newtonsoft.Json;

namespace BlinkLib
{
    public class CreateStructure : Blink
    {

        public CreateStructure() { }

        private List<Branch> _folderStructure;

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
            CreateFolder();
        }

        private void CreateFolder()
        {
            var currentPath = string.Empty;

            try
            {
                foreach (var currentBranch in _folderStructure)
                {
                    currentPath = Path.Combine(WorkingDirectory.FullName, currentBranch.Name);

                    Directory.CreateDirectory(currentPath);

                    if (currentBranch.Branches != null)
                        CreateFolder(currentBranch, currentBranch.Name);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"Directory \"{currentPath}\" is not currently accessible.");
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
                throw new BlinkException(
                    $"The path to the Directory \"{currentPath}\" contains a colon character (:) that is not a part of a drive label.");
            }
        }

        private void CreateFolder(Branch node, string rootPath)
        {
            var currentPath = string.Empty;

            try
            {
                foreach (var currentBranch in node.Branches)
                {
                    currentPath = Path.Combine(WorkingDirectory.FullName, rootPath, currentBranch.Name);

                    Directory.CreateDirectory(currentPath);

                    if (currentBranch.Branches != null)
                        CreateFolder(currentBranch, Path.Combine(rootPath, currentBranch.Name));
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"Directory \"{currentPath}\" is not currently accessible.");
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
                throw new BlinkException(
                    $"The path to the Directory \"{currentPath}\" contains a colon character (:) that is not a part of a drive label.");
            }
        }
    }
}
