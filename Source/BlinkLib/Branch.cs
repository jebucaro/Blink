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
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BlinkLib
{
    public class Branch
    {
        private string _name;

        /// <summary>
        /// Gets or sets Name of the Directory
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set
            {
                var containsABadCharacter = new Regex(
                    "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");

                if (containsABadCharacter.IsMatch(value))
                    throw new FormatException(value);

                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the label asigned to current Directory, ignored when blank or when Browsable is true
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets whether or not all files inside current directory will be listed using the asociated label
        /// </summary>
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Browsable { get; set; }

        /// <summary>
        /// Gets or sets nodes of the type Branch
        /// </summary>
        [JsonProperty("branch")]
        public List<Branch> Branches { get; set; }
    }
}