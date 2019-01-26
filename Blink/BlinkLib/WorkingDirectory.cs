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
using System.Text.RegularExpressions;

namespace BlinkLib
{
    public class WorkingDirectory
    {
        

        private Blink _blinkStrategy;

        public string Path { get; }

        public WorkingDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var containsABadCharacter = new Regex(
                "[" + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");

            if (containsABadCharacter.IsMatch(path))
                throw new FormatException($"WorkingDirectory \"{path}\" has invalid path characters.");

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"WorkingDirectory \"{path}\" doesn't exists");

            Path = path;

            ConfigurationFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigurationFile);
        }

        public string ConfigurationFile { get; set; }

        public void SetBlinkStrategy(Blink strategy)
        {
            _blinkStrategy = strategy;
        }
        
        public void Execute()
        {
            _blinkStrategy.WorkingDirectory = Path;
            _blinkStrategy.ConfigurationFile = ConfigurationFile;

            _blinkStrategy.Execute();
        }
    }
}