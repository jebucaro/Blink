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
