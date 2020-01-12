using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheRuleOfSilvester.Core.Options
{
    public sealed class OptionFile
    {
        public ConcurrentDictionary<string, Option> Options { get; set; }

        [JsonIgnore]
        public FileInfo FileInfo { get; private set; }

        public OptionFile(FileInfo fileInfo)
        {
            Options = new ConcurrentDictionary<string, Option>()
            {
                [OptionKeys.Player] = new Option("User"),
                [OptionKeys.Host] = new Option("localhost")
            };

            FileInfo = fileInfo;
        }

        public void Save()
            => File.WriteAllText(FileInfo.FullName, JsonConvert.SerializeObject(this, Formatting.Indented));

        public static OptionFile Load(FileInfo fileInfo)
        {
            var optionFileInfo = fileInfo ?? new FileInfo(Path.Combine(".", "options.json"));
            OptionFile optionFile;

            if (!optionFileInfo.Exists)
            {
                optionFile = new OptionFile(optionFileInfo);
                optionFile.Save();
            }
            else
            {
                optionFile = JsonConvert.DeserializeObject<OptionFile>(File.ReadAllText(optionFileInfo.FullName));
                optionFile.FileInfo = optionFileInfo;
            }

            return optionFile;
        }
        public static OptionFile Load()
            => Load(null);
    }
}
