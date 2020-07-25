using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink.Plugin.CommonConfiguration
{
    public class JsonConfigurationFile<T>
    {
        protected const string DefaultConfigurationFile = "Blink\\CommonConfiguration\\folder.settings.json";

        private List<T> ItemList;
        public ReadOnlyCollection<T> ReadOnlyItemList
        {
            get
            {
                return ItemList.AsReadOnly();
            }
        }

        public string ConfigurationFile { get; set; }

        public JsonConfigurationFile()
        {
            ItemList = new List<T>();

            string customConfigurationFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DefaultConfigurationFile);

            if (File.Exists(customConfigurationFile))
                ConfigurationFile = customConfigurationFile;
        }
        public void LoadConfiguration()
        {
            using (var r = new StreamReader(ConfigurationFile))
            {
                var json = r.ReadToEnd();
                ItemList = JsonConvert.DeserializeObject<List<T>>(json);
            }
        }
    }
}
