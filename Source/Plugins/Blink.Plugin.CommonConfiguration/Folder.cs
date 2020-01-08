using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace Blink.Plugin.CommonConfiguration
{
    public class Folder
    {
        private string _name;

        /// <summary>
        /// Gets or sets Name of the Directory
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set
            {
                var containsABadCharacter = new Regex(
                    "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");

                if (containsABadCharacter.IsMatch(value))
                    throw new FormatException(value);

                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the label asigned to current Directory, ignored when blank or when Browsable is false
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets whether or not all files inside current directory will be listed using the asociated label
        /// </summary>
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Browsable { get; set; }

        /// <summary>
        /// Gets or sets nodes of the type Branch
        /// </summary>
        [JsonProperty("subfolders")]
        public List<Folder> SubFolders { get; set; }
    }
}
