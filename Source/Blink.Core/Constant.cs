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
