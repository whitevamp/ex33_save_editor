using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SaveEditorApp.Helpers
    {
    public static class SaveDataLoader
        {
        public static Dictionary<string, int> LoadInventoryValues(string jsonPath)
            {
            var values = new Dictionary<string, int>();

            if (!File.Exists(jsonPath))
                return values;

            using var stream = File.OpenRead(jsonPath);
            using var doc = JsonDocument.Parse(stream);

            if (!doc.RootElement.TryGetProperty("properties", out var root))
                return values;

            if (!root.TryGetProperty("InventoryItems_0", out var inventory))
                return values;

            if (inventory.ValueKind != JsonValueKind.Array)
                return values;

            foreach (var item in inventory.EnumerateArray())
                {
                if (item.TryGetProperty("key", out var keyObj) &&
                    keyObj.TryGetProperty("Name", out var nameElem) &&
                    item.TryGetProperty("value", out var valueObj) &&
                    valueObj.TryGetProperty("Int", out var intElem))
                    {
                    var name = nameElem.GetString();
                    var intValue = intElem.GetInt32();
                    values[name] = intValue;
                    }
                }

            return values;
            }
        }
    }

