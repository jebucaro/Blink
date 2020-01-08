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

using Blink.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blink.Core
{
    public static class PluginManager
    {
        private static readonly string[] Directories = { Constant.PreinstalledDirectory, Constant.PluginsDirectory };

        private static readonly List<PluginDetail> PluginDetails = new List<PluginDetail>();

        public static List<PluginDuo> AllPlugins { get; private set; }
        public static void LoadPlugins()
        {
            if (!Directory.Exists(Constant.PluginsDirectory))
            {
                Directory.CreateDirectory(Constant.PluginsDirectory);
            }
            
            Parse(Directories);

            AllPlugins = Plugins(PluginDetails);
        }

        private static void Parse(string[] pluginDirectories)
        {
            PluginDetails.Clear();

            var directories = pluginDirectories.SelectMany(Directory.GetDirectories);
            ParsePluginDetails(directories);
        }

        private static void ParsePluginDetails(IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                PluginDetail detail = GetPluginDetail(directory);
                if (detail != null)
                {
                    PluginDetails.Add(detail);
                }
            }
        }

        private static PluginDetail GetPluginDetail(string pluginDirectory)
        {
            string configPath = Path.Combine(pluginDirectory, Constant.PluginConfigName);

            if (!File.Exists(configPath))
            {
                return null;
            }

            PluginDetail detail;

            try
            {
                detail = JsonConvert.DeserializeObject<PluginDetail>(File.ReadAllText(configPath));
                #if DEBUG
                detail.PluginDirectory = Path.Combine(pluginDirectory, @"bin\Debug");
                #else
                detail.PluginDirectory = pluginDirectory;
                #endif
            }
            catch (Exception)
            {
                return null;
            }

            if (!File.Exists(detail.PluginFilePath))
            {
                return null;
            }

            return detail;
        }

        private static List<PluginDuo> Plugins(List<PluginDetail> details)
        {
            var dotNetPlugins = GetDotNetPlugins(details).ToList();

            return dotNetPlugins;
        }

        private static IEnumerable<PluginDuo> GetDotNetPlugins(List<PluginDetail> source)
        {
            var plugins = new List<PluginDuo>();
            var details = source;

            foreach (var detail in details)
            {
                var assembly = Assembly.Load(AssemblyName.GetAssemblyName(detail.PluginFilePath));
                var types = assembly.GetTypes();
                var type = types.First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(IBlink)));

                var plugin = (IBlink)Activator.CreateInstance(type);

                PluginDuo pair = new PluginDuo
                {
                    Plugin = plugin,
                    Detail = detail
                };
                plugins.Add(pair);
            }

            return plugins;
        }

        public static void InitializePlugins()
        {
            Parallel.ForEach(AllPlugins, pair =>
            {
                pair.Plugin.Init(pair.Detail);
            });
        }

        public static void ExecutePlugin(PluginDuo duo)
        {
            try
            {
                duo.Plugin.ExecuteTask();
            }
            catch (BlinkException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static PluginDuo GetPluginForId(string id)
        {
            return AllPlugins.FirstOrDefault(o => o.Detail.Id == id);
        }

        public static PluginDuo GetPluginForActionKeyword(string actionKeyword)
        {
            return AllPlugins.FirstOrDefault(o => o.Detail.ActionKeyword == actionKeyword);
        }
    }
}
