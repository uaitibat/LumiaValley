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

        public bool IsMale { get; set; }
        public int Skin { get; set; }
        public int Hair { get; set; }
        public int HairColor { get; set; }
        public int Shirt { get; set; }
        public int ShirtColor { get; set; }
        public int Pants { get; set; }
        public int PantsColor { get; set; }
        public int Shoes { get; set; }
        public int Accessory { get; set; }
        public int EyeColor { get; set; }
        public int Facing { get; set; }
        public int Pet { get; set; }
        public int FarmType { get; set; }
        public string FavoriteThing { get; set; }
        public bool SkipIntro { get; set; }

        // Opcoes do painel Avancado (AdvancedGameOptions da 1.5.6).
        public bool SpawnMonstersAtNight { get; set; }
        public bool YearOneCompletable { get; set; }
        public bool UseRandomSeed { get; set; }

        public SaveSlot()
        {
            FarmerName = "Fazendeiro";
            FarmName = "Fazenda";
            Created = string.Empty;
            FavoriteThing = string.Empty;
            IsMale = true;
            SkipIntro = false;
        }
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

        /// <summary>Cria um save com os valores escolhidos na customizacao.</summary>
        public static void Create(SaveSlot slot)
        {
            if (slot == null)
                return;

            try
            {
                var name = "save" + DateTime.Now.Ticks + Extension;
                var text =
                    "version=2\n" +
                    "farmer=" + Clean(slot.FarmerName) + "\n" +
                    "farm=" + Clean(slot.FarmName) + "\n" +
                    "created=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\n" +
                    "male=" + Flag(slot.IsMale) + "\n" +
                    "skin=" + slot.Skin + "\n" +
                    "hair=" + slot.Hair + "\n" +
                    "haircolor=" + slot.HairColor + "\n" +
                    "shirt=" + slot.Shirt + "\n" +
                    "shirtcolor=" + slot.ShirtColor + "\n" +
                    "pants=" + slot.Pants + "\n" +
                    "pantscolor=" + slot.PantsColor + "\n" +
                    "shoes=" + slot.Shoes + "\n" +
                    "accessory=" + slot.Accessory + "\n" +
                    "eyecolor=" + slot.EyeColor + "\n" +
                    "facing=" + slot.Facing + "\n" +
                    "pet=" + slot.Pet + "\n" +
                    "farmtype=" + slot.FarmType + "\n" +
                    "favorite=" + Clean(slot.FavoriteThing) + "\n" +
                    "skipintro=" + Flag(slot.SkipIntro) + "\n";

                var file = Folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting)
                    .AsTask().GetAwaiter().GetResult();
                FileIO.WriteTextAsync(file, text).AsTask().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao criar save: " + ex.Message);
            }
        }

        /// <summary>Mantido para saves de teste criados antes da customizacao.</summary>
        public static void Create(string farmerName, string farmName)
        {
            Create(new SaveSlot { FarmerName = farmerName, FarmName = farmName });
        }

        private static SaveSlot Parse(string fileName, string text)
        {
            var slot = new SaveSlot
            {
                FileName = fileName
            };

            if (string.IsNullOrEmpty(text))
                return slot;

            foreach (var raw in text.Split('\n'))
            {
                var line = raw.Trim();
                var index = line.IndexOf('=');
                if (index < 0)
                    continue;

                var key = line.Substring(0, index);
                var value = line.Substring(index + 1);

                switch (key)
                {
                    case "farmer": slot.FarmerName = value; break;
                    case "farm": slot.FarmName = value; break;
                    case "created": slot.Created = value; break;
                    case "male": slot.IsMale = Bool(value, true); break;
                    case "skin": slot.Skin = Int(value, 0, 0, 23); break;
                    case "hair": slot.Hair = Int(value, 0, 0, 111); break;
                    case "haircolor": slot.HairColor = Int(value, 0, 0, 11); break;
                    case "shirt": slot.Shirt = Int(value, 0, 0, 199); break;
                    case "shirtcolor": slot.ShirtColor = Int(value, 0, 0, 19); break;
                    case "pants": slot.Pants = Int(value, 0, 0, 19); break;
                    case "pantscolor": slot.PantsColor = Int(value, 0, 0, 19); break;
                    case "shoes": slot.Shoes = Int(value, 0, 0, 18); break;
                    case "accessory": slot.Accessory = Int(value, -1, -1, 19); break;
                    case "eyecolor": slot.EyeColor = Int(value, 0, 0, 11); break;
                    case "facing": slot.Facing = Int(value, 0, 0, 3); break;
                    case "pet": slot.Pet = Int(value, 0, 0, 2); break;
                    case "farmtype": slot.FarmType = Int(value, 0, 0, FarmTypes.Count - 1); break;
                    case "favorite": slot.FavoriteThing = value; break;
                    case "skipintro": slot.SkipIntro = Bool(value, true); break;
                }
            }

            return slot;
        }

        private static int Int(string value, int fallback, int min, int max)
        {
            int result;
            if (!int.TryParse(value, out result))
                return fallback;
            if (result < min)
                return min;
            if (result > max)
                return max;
            return result;
        }

        private static bool Bool(string value, bool fallback)
        {
            if (value == "1")
                return true;
            if (value == "0")
                return false;
            return fallback;
        }

        private static string Flag(bool value)
        {
            return value ? "1" : "0";
        }

        private static string Clean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            if (value.Length > 24)
                value = value.Substring(0, 24);
            return value.Replace('\n', ' ').Replace('\r', ' ').Replace('=', '-');
        }
    }
}
