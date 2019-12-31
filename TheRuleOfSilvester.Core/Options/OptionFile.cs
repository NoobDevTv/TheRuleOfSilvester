using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheRuleOfSilvester.Core.Options
{
    public sealed class OptionFile
    {
        public Dictionary<string, Option> Options { get; set; }

        private readonly FileInfo fileInfo;

        public OptionFile(FileInfo fileInfo)
        {
            Options = new Dictionary<string, Option>();
            this.fileInfo = fileInfo;
        }

        public void Save() 
            => File.WriteAllText(fileInfo.FullName, JsonConvert.SerializeObject(this, Formatting.Indented));

        public static OptionFile Load(FileInfo fileInfo)
        {
            var optionFileInfo = fileInfo;
            OptionFile optionFile;

            if (!optionFileInfo.Exists)
            {
                optionFile = new OptionFile(fileInfo);
                optionFile.Save();
            }
            else
            {
                optionFile = JsonConvert.DeserializeObject<OptionFile>(File.ReadAllText(optionFileInfo.FullName));
            }

            return optionFile;
        }
        public static OptionFile Load()
            => Load(null);
    }
}
