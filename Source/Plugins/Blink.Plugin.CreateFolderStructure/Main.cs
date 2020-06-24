﻿using Blink.Plugin.CommonConfiguration;
using System;
using System.IO;

namespace Blink.Plugin.CreateFolderStructure
{
    public class Main : JsonConfigurationFile<Folder>, IBlink
    {
        private const string DefaultConfigurationFile = "folder.settings.json";
        public DirectoryInfo WorkingDirectory { get; set; }

        /// <summary>
        /// Creates a folder structure based on configuration file
        /// </summary>
        public void ExecuteTask()
        {
            try
            {
                LoadConfiguration();
            }
            catch (FileNotFoundException)
            {
                throw new BlinkException($"Unable to find configuration file: \"{ConfigurationFile}\".");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException(
                    $"Unable to access Blink application directory \"{AppDomain.CurrentDomain.BaseDirectory}\".");
            }
            catch (FormatException ex)
            {
                throw new BlinkException(
                    $"Invalid Directory name: \"{ex.Message}\", check your \"{ConfigurationFile}\" file.");
            }
            catch (Exception ex)
            {
                throw new BlinkException(
                    $"There was an error while parsing configuration file: \"{ConfigurationFile}\".", ex);
            }

            CreateFolders();
        }

        public void Init(PluginDetail pluginDetail)
        {
            this.ConfigurationFile = Path.Combine(pluginDetail.PluginDirectory, DefaultConfigurationFile);
        }

        /// <summary>
        /// Creates a folder structure based on configuration file
        /// </summary>
        private void CreateFolders()
        {
            var currentPath = string.Empty;

            try
            {
                foreach (var currentFolder in ReadOnlyItemList)
                {
                    currentPath = Path.Combine(WorkingDirectory.FullName, currentFolder.Name);

                    Directory.CreateDirectory(currentPath);

                    if (currentFolder.SubFolders != null)
                        CreateFolders(currentFolder, currentFolder.Name);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"Directory \"{currentPath}\" is not currently accessible.");
            }
            catch (PathTooLongException)
            {
                throw new BlinkException($"The path to the Directory \"{currentPath}\" is too long");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException($"Directory not found \"{currentPath}\".");
            }
            catch (NotSupportedException)
            {
                throw new BlinkException(
                    $"The path to the Directory \"{currentPath}\" contains a colon character (:) that is not a part of a drive label.");
            }
        }

        /// <summary>
        /// Creates a folder structure based on configuration file
        /// </summary>
        /// <param name="folder">Current node</param>
        /// <param name="rootPath">Parent Directory path</param>
        private void CreateFolders(Folder folder, string rootPath)
        {
            var currentPath = string.Empty;

            try
            {
                foreach (var currentFolder in folder.SubFolders)
                {
                    currentPath = Path.Combine(WorkingDirectory.FullName, rootPath, currentFolder.Name);

                    Directory.CreateDirectory(currentPath);

                    if (currentFolder.SubFolders != null)
                        CreateFolders(currentFolder, Path.Combine(rootPath, currentFolder.Name));
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new BlinkException($"Directory \"{currentPath}\" is not currently accessible.");
            }
            catch (PathTooLongException)
            {
                throw new BlinkException($"The path to the Directory \"{currentPath}\" is too long");
            }
            catch (DirectoryNotFoundException)
            {
                throw new BlinkException($"Directory not found \"{currentPath}\".");
            }
            catch (NotSupportedException)
            {
                throw new BlinkException(
                    $"The path to the Directory \"{currentPath}\" contains a colon character (:) that is not a part of a drive label.");
            }
        }
    }
}
