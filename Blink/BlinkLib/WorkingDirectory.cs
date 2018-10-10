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
        public string Path { get; private set; }

        public string Name { get; private set; }

        public WorkingDirectory(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("WorkingDirectory is empty.");

            Regex containsABadCharacter = new Regex(
                "[" + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");

            if (containsABadCharacter.IsMatch(path))
                throw new FormatException(String.Format("WorkingDirectory \"{0}\" has invalid path characters.", path));

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException(String.Format("WorkingDirectory \"{0}\" doesn't exists", path));

            this.Path = path;

            //this.Name = System.IO.Path.GetFileName(this.Path);
        }
    }
}
