/*
 * MIT License
 * 
 * Copyright (c) 2019 Jonathan Búcaro
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
        /// <summary>
        /// PluginId
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Keyword used to launch the Plugin
        /// </summary>
        public string ActionKeyword { get; set; }
        /// <summary>
        /// Name of the Plugin
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Small description of the what the Plugin does
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Name of the developer who made the Plugin
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Version of the Plugin
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Developer's Website
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// Name of the compiled binary of the Plugin
        /// </summary>
        public string PluginFileName { get; set; }
        /// <summary>
        /// Full path to the compiled binary of the Plugin
        /// </summary>
        public string PluginFilePath { get; private set; }

        private string _pluginDirectory;
        /// <summary>
        /// Directory where the compiled binary of the Plugin is located
        /// </summary>
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
            }
        }
        /// <summary>
        /// Returns the Name of the Plugin
        /// </summary>
        /// <returns>Name of the Plugin</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
