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
    public abstract class Blink
    {

        private const string DefaultConfigurationFile = "branch.settings.json";

        protected List<Branch> FolderStructure;

        /// <summary>
        /// Gets or Sets the path to the configuration file used by all Blink subclasses
        /// </summary>
        public string ConfigurationFile { get; set; }

        /// <summary>
        /// Gets or Sets the base Directory used by all Blink subclasses
        /// </summary>
        public DirectoryInfo WorkingDirectory { get; set; }

        /// <summary>
        /// Loads configuration file in a Blink subclass
        /// </summary>
        protected virtual void LoadConfiguration()
        {
            try
            {
                using (var r = new StreamReader(ConfigurationFile))
                {
                    var json = r.ReadToEnd();
                    FolderStructure = JsonConvert.DeserializeObject<List<Branch>>(json);
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

        /// <summary>
        /// Executes main task in a Blink subclass
        /// </summary>
        protected abstract void ExecuteTask();

        /// <summary>
        /// Creates a new instance of Blink class
        /// </summary>
        protected Blink()
        {
            ConfigurationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigurationFile);
        }

        /// <summary>
        /// Starts the process associated with a Blink class
        /// </summary>
        public void Execute()
        {
            if (WorkingDirectory is null)
                throw new BlinkException(
                    $"Working Directory is not assigned.");

            if (!WorkingDirectory.Exists)
                throw new BlinkException(
                    $"Working Directory does not exists \"{WorkingDirectory.FullName}\".");
                
            LoadConfiguration();

            ExecuteTask();
        }
    }
}
