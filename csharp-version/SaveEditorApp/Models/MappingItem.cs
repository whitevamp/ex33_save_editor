using YamlDotNet.Serialization;

namespace SaveEditorApp.Models
    {
    public class MappingItem
        {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "save_key")]
        public string SaveKey { get; set; }

        [YamlMember(Alias = "category")]
        public string Category { get; set; }

        [YamlMember(Alias = "type")]
        public string Type { get; set; }
        }
    }


