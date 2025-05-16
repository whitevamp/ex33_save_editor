using SaveEditorApp.Models;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SaveEditorApp.Helpers
    {
    public static class YamlMappingLoader
        {
        public static List<MappingItem> LoadAllMappings(string mappingsFolder)
            {
            var allItems = new List<MappingItem>();
            //var deserializer = new DeserializerBuilder()
            //    .WithNamingConvention(CamelCaseNamingConvention.Instance)
            //    .Build();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();


            foreach (var file in Directory.GetFiles(mappingsFolder, "*.yaml"))
                {
                var content = File.ReadAllText(file);
                var doc = deserializer.Deserialize<YamlRoot>(content);
                if (doc?.Items != null)
                    allItems.AddRange(doc.Items);
                }

            return allItems;
            }

        private class YamlRoot
            {
            private List<MappingItem> items;

            public List<MappingItem> Items { get => items; set => items = value; }
            }
        }
    }

