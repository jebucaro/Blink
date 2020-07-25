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
using System;
using System.IO;

namespace Blink.Plugin.FolderStructure
{
    public class Main : JsonConfigurationFile<Folder>, IBlink
    {
        private const string DefaultConfigurationFile = "folder.settings.json";
        public DirectoryInfo WorkingDirectory { get; set; }

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

            CreateFolders();
        }

        public void Init(PluginDetail pluginDetail)
        {
            this.ConfigurationFile = Path.Combine(pluginDetail.PluginDirectory, DefaultConfigurationFile);
        }

        private void CreateFolders()
        {
            var currentPath = string.Empty;

            try
            {
                foreach (var currentFolder in ReadOnlyItemList)
                {
                    currentPath = Path.Combine(WorkingDirectory.FullName, currentFolder.Name);

                    Directory.CreateDirectory(currentPath);

                    if (currentFolder.SubFolders != null)
                        CreateFolders(currentFolder, currentFolder.Name);
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

        private void CreateFolders(Folder folder, string rootPath)
        {
            var currentPath = string.Empty;

            try
            {
                foreach (var currentFolder in folder.SubFolders)
                {
                    currentPath = Path.Combine(WorkingDirectory.FullName, rootPath, currentFolder.Name);

                    Directory.CreateDirectory(currentPath);

                    if (currentFolder.SubFolders != null)
                        CreateFolders(currentFolder, Path.Combine(rootPath, currentFolder.Name));
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
