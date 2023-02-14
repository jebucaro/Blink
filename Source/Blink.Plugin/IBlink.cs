using System.IO;

namespace Blink.Plugin
{
    public interface IBlink
    {
        DirectoryInfo WorkingDirectory { get; set; }

        void Init(PluginDetail pluginDetail);
        void ExecuteTask();
    }
}
