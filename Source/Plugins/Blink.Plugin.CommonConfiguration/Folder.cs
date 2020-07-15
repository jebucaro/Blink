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

        [JsonProperty("label")]
        public string Label { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Browsable { get; set; }

        [JsonProperty("subfolders")]
        public List<Folder> SubFolders { get; set; }
    }
}
