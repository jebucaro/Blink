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
    public class PluginManager
    {
        private readonly string[] Directories = { Constant.PreinstalledDirectory, Constant.PluginsDirectory };

        public IReadOnlyList<PluginDetail> AvailablePlugins { get; private set; }
        private readonly List<PluginDuo> AllPlugins = new List<PluginDuo>();

        public enum PluginSearchType
        {
            SearchById,
            SearchByActionKeyword
        }

        public void LoadPlugins()
        {
            AllPlugins.Clear();

            if (!Directory.Exists(Constant.PluginsDirectory))
            {
                Directory.CreateDirectory(Constant.PluginsDirectory);
            }

            var pluginsInstalled = Parse();

            AllPlugins.AddRange(GetPlugins(pluginsInstalled));

            AvailablePlugins = AllPlugins.Select(o => o.Detail).ToList().AsReadOnly();
        }

        private IEnumerable<PluginDetail> Parse()
        {
            var directories = Directories.SelectMany(Directory.GetDirectories);

            foreach (var directory in directories)
            {
                PluginDetail detail = GetPluginDetail(directory);
                if (detail != null)
                {
                    yield return detail;
                }
            }
        }

        private PluginDetail GetPluginDetail(string pluginDirectory)
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

        private IEnumerable<PluginDuo> GetPlugins(IEnumerable<PluginDetail> pluginDetails)
        {
            foreach (var detail in pluginDetails)
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

                yield return pair;
            }
        }

        public void InitializePlugins()
        {
            Parallel.ForEach(AllPlugins, pair =>
            {
                pair.Plugin.Init(pair.Detail);
            });
        }

        public void InitializePlugin(PluginSearchType pluginSearchType, string value)
        {
            var duo = pluginSearchType ==
                    PluginSearchType.SearchById ?
                        GetPluginForId(value) : GetPluginForActionKeyword(value);

            duo.Plugin.Init(duo.Detail);
        }

        public void ExecutePlugin(PluginSearchType pluginSearchType, string value, string path)
        {
            var duo = pluginSearchType ==
                    PluginSearchType.SearchById ?
                        GetPluginForId(value) : GetPluginForActionKeyword(value);

            try
            {
                duo.Plugin.WorkingDirectory = new DirectoryInfo(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            duo.Plugin.ExecuteTask();
        }

        private PluginDuo GetPluginForId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            return AllPlugins.FirstOrDefault(o => o.Detail.Id == id);
        }

        private PluginDuo GetPluginForActionKeyword(string actionKeyword)
        {
            if (string.IsNullOrWhiteSpace(actionKeyword))
                throw new ArgumentNullException(nameof(actionKeyword));

            return AllPlugins.FirstOrDefault(o => o.Detail.ActionKeyword == actionKeyword);
        }
    }
}
