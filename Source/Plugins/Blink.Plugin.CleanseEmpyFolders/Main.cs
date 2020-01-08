using System;
using System.IO;
using System.Linq;

namespace Blink.Plugin.CleanseEmpyFolders
{
    public class Main : IBlink
    {
        public DirectoryInfo WorkingDirectory { get; set; }

        /// <summary>
        /// Cleanse empty folders
        /// </summary>
        public void ExecuteTask()
        {
            CleanseFolder(WorkingDirectory.FullName);
        }

        public void Init(PluginDetail pluginDetail)
        {
            //
        }

        /// <summary>
        /// Cleanse empty folders
        /// </summary>
        /// <param name="startLocation">Directory that is going to be cleansed</param>
        private void CleanseFolder(string startLocation)
        {
            var folder = startLocation;

            try
            {
                foreach (var currentDirectory in Directory.GetDirectories(startLocation))
                {
                    folder = currentDirectory;

                    CleanseFolder(currentDirectory);

                    if (!Directory.EnumerateFileSystemEntries(currentDirectory).Any())
                        Directory.Delete(currentDirectory, false);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"Directory \"{folder}\" is not currently accessible.");
            }
            catch (PathTooLongException)
            {
                throw new BlinkException($"The path to the Directory \"{folder}\" is too long.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException($"Directory not found \"{folder}\".");
            }
        }
    }
}
