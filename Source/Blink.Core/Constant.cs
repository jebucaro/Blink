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

using System;
using System.IO;
using System.Reflection;

namespace Blink.Core
{
    static class Constant
    {
        public const string Blink = "Blink";
        public const string Plugins = "Plugins";
        public const string PluginConfigName = "plugin.json";

        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        public static readonly string ProgramDirectory = Directory.GetParent(Assembly.Location).ToString();
        public static readonly string ExecutablePath = Path.Combine(ProgramDirectory, Blink + ".exe");
        public static readonly string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Blink);
        public static readonly string PluginsDirectory = Path.Combine(DataDirectory, Plugins);
#if DEBUG
        public static readonly string PreinstalledDirectory = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(ProgramDirectory).ToString()).ToString()).ToString(), Plugins);
#else
        public static readonly string PreinstalledDirectory = Path.Combine(ProgramDirectory, Plugins);
#endif

    }
}
