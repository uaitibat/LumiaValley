using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Search;

namespace LumiaValley.Game
{
    /// <summary>
    /// Save proprio do port: arquivos de texto simples no LocalFolder do app.
    /// Nao e compativel com os saves XML do PC (decisao registrada no roadmap).
    /// </summary>
    public sealed class SaveSlot
    {
        public string FileName { get; set; }
        public string FarmerName { get; set; }
        public string FarmName { get; set; }
        public string Created { get; set; }
    }

    public static class SaveStore
    {
        private const string Extension = ".lvsav";

        private static StorageFolder Folder
        {
            get { return ApplicationData.Current.LocalFolder; }
        }

        public static List<SaveSlot> List()
        {
            var result = new List<SaveSlot>();
            try
            {
                var query = Folder.CreateFileQuery(CommonFileQuery.OrderByName);
                var files = query.GetFilesAsync().AsTask().GetAwaiter().GetResult();
                foreach (var file in files)
                {
                    if (!file.Name.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var text = FileIO.ReadTextAsync(file).AsTask().GetAwaiter().GetResult();
                    result.Add(Parse(file.Name, text));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao listar saves: " + ex.Message);
            }
            return result;
        }

        public static void Create(string farmerName, string farmName)
        {
            try
            {
                var name = "save" + DateTime.Now.Ticks + Extension;
                var text =
                    "version=1\n" +
                    "farmer=" + Clean(farmerName) + "\n" +
                    "farm=" + Clean(farmName) + "\n" +
                    "created=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\n";

                var file = Folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting)
                    .AsTask().GetAwaiter().GetResult();
                FileIO.WriteTextAsync(file, text).AsTask().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao criar save: " + ex.Message);
            }
        }

        private static SaveSlot Parse(string fileName, string text)
        {
            var slot = new SaveSlot
            {
                FileName = fileName,
                FarmerName = "Farmer",
                FarmName = "Farm",
                Created = string.Empty
            };

            if (string.IsNullOrEmpty(text))
                return slot;

            foreach (var line in text.Split('\n'))
            {
                var trimmed = line.Trim();
                var index = trimmed.IndexOf('=');
                if (index < 0)
                    continue;

                var key = trimmed.Substring(0, index);
                var value = trimmed.Substring(index + 1);
                if (key == "farmer") slot.FarmerName = value;
                else if (key == "farm") slot.FarmName = value;
                else if (key == "created") slot.Created = value;
            }

            return slot;
        }

        private static string Clean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Replace('\n', ' ').Replace('\r', ' ').Replace('=', '-');
        }
    }
}
