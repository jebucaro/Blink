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

namespace Blink.Plugin
{
    public class PluginDetail
    {
        public string Id { get; set; }
        public string ActionKeyword { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Website { get; set; }
        public string PluginIcon { get; set; }
        public string PluginIconPath { get; private set; }
        public string PluginFileName { get; set; }
        public string PluginFilePath { get; private set; }

        private string _pluginDirectory;
        
        public string PluginDirectory
        {
            get
            {
                return _pluginDirectory;
            }
            internal set
            {
                _pluginDirectory = value;
                PluginFilePath = System.IO.Path.Combine(value, PluginFileName);
                PluginIconPath = System.IO.Path.Combine(value, PluginIcon);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
