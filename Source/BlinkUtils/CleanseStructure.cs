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
using System.IO;
using System.Linq;
using BlinkLib;

namespace BlinkClient
{
    public sealed class CleanseStructure : Blink
    {
        /// <summary>
        /// Creates a new instance of CleanseStructure
        /// </summary>
        public CleanseStructure(){ }

        /// <summary>
        /// Loads the configuration file
        /// </summary>
        protected override void LoadConfiguration()
        {
            // There is no configuration for this task :)
        }

        /// <summary>
        /// Cleanse empty folders
        /// </summary>
        protected override void ExecuteTask()
        {
            CleanseFolder(WorkingDirectory.FullName);
        }

        /// <summary>
        /// Cleanse empty folders
        /// </summary>
        /// <param name="startLocation">Directory that is going to be cleansed</param>
        private void CleanseFolder(string startLocation)
        {
            var folder = startLocation;

            try
            {
                foreach (var currentDirectory in Directory.GetDirectories(startLocation))
                {
                    folder = currentDirectory;

                    CleanseFolder(currentDirectory);

                    if (!Directory.EnumerateFileSystemEntries(currentDirectory).Any())
                        Directory.Delete(currentDirectory, false);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"Directory \"{folder}\" is not currently accessible.");
            }
            catch (PathTooLongException)
            {
                throw new BlinkException($"The path to the Directory \"{folder}\" is too long.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException($"Directory not found \"{folder}\".");
            }
        }
    }
}
