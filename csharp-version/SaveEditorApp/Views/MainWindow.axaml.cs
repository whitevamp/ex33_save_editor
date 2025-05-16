// File: MainWindow.xaml.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using SaveEditorApp.Helpers;
using SaveEditorApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace SaveEditorApp
    {
    public partial class MainWindow : Window
        {

        private Dictionary<string, int> _saveValues = new();
        private string? _lastJsonPath = null;
        private List<TextBox> _editableFields = new();
        public MainWindow()
            {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            // Hook up menu events
            this.FindControl<MenuItem>("OpenSaveMenuItem").Click += OpenSaveMenuItem_Click;
            this.FindControl<MenuItem>("ExportSaveMenuItem").Click += ExportSaveMenuItem_Click;
            this.FindControl<MenuItem>("ExitMenuItem").Click += ExitMenuItem_Click;

            }

        private void InitializeComponent()
            {
            AvaloniaXamlLoader.Load(this);

            // After load, populate the TreeView
            PopulateTreeView();
            }

        private async void OpenSaveMenuItem_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
            var openPickerOptions = new FilePickerOpenOptions
                {
                Title = "Open Save File",
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new FilePickerFileType("Save Files") { Patterns = new[] { "*.sav" } }
                },
                AllowMultiple = false
                };

            var result = await this.StorageProvider.OpenFilePickerAsync(openPickerOptions);
            if (result != null && result.Count > 0)
                {
                var file = result[0];
                await ShowMessage($"Selected save file:\n{file.Path.LocalPath}");

                var jsonPath = file.Path.LocalPath;

                // TEMP: assume user selects an already unpacked .json file
                _saveValues = SaveDataLoader.LoadInventoryValues(jsonPath);
                _lastJsonPath = jsonPath;

                await ShowMessage($"Loaded {_saveValues.Count} values from:\n{jsonPath}");
                }
            }

        // Updated ExportSaveMenuItem_Click method to use the StorageProvider API
        //private async void ExportSaveMenuItem_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        //    {
        //    var savePickerOptions = new FilePickerSaveOptions
        //        {
        //        Title = "Export Save File",
        //        FileTypeChoices = new List<FilePickerFileType>
        //        {
        //            new FilePickerFileType("Save Files") { Patterns = new[] { "*.sav" } }
        //        }
        //        };

        //    var file = await this.StorageProvider.SaveFilePickerAsync(savePickerOptions);
        //    if (file != null)
        //        {
        //        await ShowMessage($"Export path:\n{file.Path.LocalPath}");
        //        }
        //    }
        private async void ExportSaveMenuItem_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
            if (_lastJsonPath == null)
                {
                await ShowMessage("No save loaded yet.");
                return;
                }

            var savePickerOptions = new FilePickerSaveOptions
                {
                Title = "Export Save File",
                FileTypeChoices = new List<FilePickerFileType>
        {
            new FilePickerFileType("Save Files") { Patterns = new[] { "*.json" } }
        }
                };

            var file = await this.StorageProvider.SaveFilePickerAsync(savePickerOptions);
            if (file == null)
                return;

            // Load the original file structure
            var jsonText = File.ReadAllText(_lastJsonPath);
            var doc = JsonDocument.Parse(jsonText);
            var root = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonText)!;

            // Convert editable fields into a key-value dictionary
            var updatedValues = new Dictionary<string, int>();
            foreach (var textBox in _editableFields)
                {
                if (textBox.Tag is string key && int.TryParse(textBox.Text, out var value))
                    {
                    updatedValues[key] = value;
                    }
                }

            // Patch InventoryItems_0 section
            if (root.TryGetValue("properties", out var propObj) &&
                propObj is JsonElement properties &&
                properties.TryGetProperty("InventoryItems_0", out var inventoryArr) &&
                inventoryArr.ValueKind == JsonValueKind.Array)
                {
                var newInventory = new List<Dictionary<string, object>>();

                foreach (var item in inventoryArr.EnumerateArray())
                    {
                    var key = item.GetProperty("key").GetProperty("Name").GetString();
                    if (key != null && updatedValues.TryGetValue(key, out var newValue))
                        {
                        newInventory.Add(new Dictionary<string, object>
                            {
                            ["key"] = new Dictionary<string, object> { ["Name"] = key },
                            ["value"] = new Dictionary<string, object> { ["Int"] = newValue }
                            });
                        }
                    else
                        {
                        // Keep original if not updated
                        newInventory.Add(JsonSerializer.Deserialize<Dictionary<string, object>>(item.GetRawText())!);
                        }
                    }

                // Replace InventoryItems_0 in root (you may need a deeper JSON merge logic here)
                var newRoot = new Dictionary<string, object>
                    {
                    ["properties"] = new Dictionary<string, object>
                        {
                        ["InventoryItems_0"] = newInventory
                        }
                    };

                var newJson = JsonSerializer.Serialize(newRoot, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(file.Path.LocalPath, newJson);

                await ShowMessage("Save file exported successfully.");
                }
            else
                {
                await ShowMessage("Could not find InventoryItems_0 in the loaded save.");
                }
            }

        private void ExitMenuItem_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
            this.Close();
            }

        private async System.Threading.Tasks.Task ShowMessage(string text)
            {
            var msgBox = new Window
                {
                Title = "Info",
                Width = 400,
                Height = 200,
                Content = new TextBlock
                    {
                    Text = text,
                    Margin = new Thickness(10),
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    }
                };
            await msgBox.ShowDialog(this);
            }

        private void PopulateTreeView()
            {
            var tree = this.FindControl<TreeView>("TreeNav");

            var items = Helpers.YamlMappingLoader.LoadAllMappings("Mappings");

            if (items.Count == 0)
                {
                Console.WriteLine("⚠ No YAML items loaded!");
                Debug.WriteLine($"⚠ No YAML items loaded!");
                }
            else
                {
                Console.WriteLine($"✅ Loaded {items.Count} items from YAML.");
                Debug.WriteLine($"✅ Loaded {items.Count} items from YAML.");
                foreach (var item in items)
                    {
                    Console.WriteLine($"- {item.Name} → {item.Category}");
                    }
                }

            tree.ItemsSource = BuildCategoryTree(items);
            }

        private List<TreeViewItem> BuildCategoryTree(List<MappingItem> items)
            {
            var categories = items
                .Where(i => i.Category?.Contains('.') == true)
                .GroupBy(i => i.Category.Split('.')[0])
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var parent = new TreeViewItem { Header = g.Key };

                    var subGroups = g.GroupBy(i => i.Category.Split('.')[1])
                                     .OrderBy(sg => sg.Key);

                    var subCategoryItems = subGroups.Select(sub =>
                        new TreeViewItem
                            {
                            Header = sub.Key,
                            Tag = $"{g.Key}.{sub.Key}" // Save full category path for selection
                            }).ToList();

                    parent.ItemsSource = subCategoryItems;

                    return parent;
                });

            return categories.ToList();
            }


        private void TreeView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
            {

            if ((e.AddedItems as IEnumerable<object>)?.FirstOrDefault() is not TreeViewItem selected)
                return;

            var categoryPath = selected.Tag?.ToString();
            if (string.IsNullOrWhiteSpace(categoryPath))
                return;

            var items = Helpers.YamlMappingLoader.LoadAllMappings("Mappings")
                .Where(i => i.Category == categoryPath)
                .OrderBy(i => i.Name)
                .ToList();

            // Display items in right panel as before...

            var rightPanel = this.FindControl<StackPanel>("RightContentPanel");
            rightPanel.Children.Clear();

            var header = new TextBlock
                {
                Text = $"Items in: {categoryPath}",
                FontSize = 18,
                Margin = new Thickness(10, 5)
                };
            rightPanel.Children.Add(header);

            // ✅ Clear previous text field references
            _editableFields.Clear();

            foreach (var item in items)
                {
                var stack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };

                stack.Children.Add(new TextBlock
                    {
                    Text = item.Name,
                    Width = 200,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    });

                var textBox = new TextBox
                    {
                    Text = _saveValues.TryGetValue(item.SaveKey, out var val) ? val.ToString() : "0",
                    Width = 80,
                    Tag = item.SaveKey
                    };

                _editableFields.Add(textBox); // 👈 this is why we clear before
                stack.Children.Add(textBox);
                rightPanel.Children.Add(stack);
                }
            }

        }

    }
