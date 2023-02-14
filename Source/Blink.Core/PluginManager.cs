using Blink.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blink.Core
{
    public class PluginManager
    {
        public enum SearchType
        {
            Id,
            ActionKeyword
        }

        private readonly string[] Directories = { Constant.PreinstalledDirectory, Constant.PluginsDirectory };

        public IReadOnlyList<PluginDetail> AvailablePlugins { get; private set; }
        private readonly List<PluginDuo> AllPlugins = new List<PluginDuo>();

        private PluginDuo selectedPlugin;
        public SearchType PluginSearchType { get; set; } = SearchType.ActionKeyword;
        

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

        public PluginDetail SelectPlugin(string value)
        {
            selectedPlugin = PluginSearchType ==
                    SearchType.Id ?
                        GetPluginForId(value) : GetPluginForActionKeyword(value);

            if (selectedPlugin is null)
                throw new KeyNotFoundException($"There is no plugin with { this.PluginSearchType }: { value }");

            return selectedPlugin.Detail;
        }

        public void InitializePlugin()
        {
            if (selectedPlugin is null)
                throw new NullReferenceException("There is no plugin selected");

            selectedPlugin.Plugin.Init(selectedPlugin.Detail);
        }

        public void ExecutePlugin(string path)
        {
            if (selectedPlugin is null)
                throw new NullReferenceException("There is no plugin selected");

            try
            {
                selectedPlugin.Plugin.WorkingDirectory = new DirectoryInfo(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            selectedPlugin.Plugin.ExecuteTask();
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
