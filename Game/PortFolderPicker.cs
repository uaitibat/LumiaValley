using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace LumiaValley.Game
{
    /// <summary>
    /// Abre o seletor e valida a pasta usando GetFileAsync direto em arquivos
    /// conhecidos. CreateFileQueryWithOptions falha em cartoes SD removiveis;
    /// GetFileAsync funciona com a permissao do FutureAccessList.
    /// </summary>
    public static class PortFolderPicker
    {
        // Arquivos que SEMPRE existem no Content do Stardew 1.5.6.
        private static readonly string[] ProbeFiles =
        {
            "Maps\\FarmHouse.xnb",
            "Data\\Furniture.xnb",
            "LooseSprites\\shadow.xnb",
            "Fonts\\SmallFont.pt-BR.xnb",
            "Characters\\Farmer\\farmer_base.xnb"
        };

        public static async Task<bool> PickAsync()
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List
            };
            picker.FileTypeFilter.Add("*");

            StorageFolder picked = null;
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                picked = await picker.PickSingleFolderAsync();
            });

            if (picked == null)
            {
                PortContentManager.SetAssetCount(0);
                return false;
            }

            // Testa a propria pasta e, se falhar, a subpasta Content.
            StorageFolder root = picked;
            var found = await ProbeCount(root);
            if (found == 0)
            {
                var sub = await TryFolder(picked, "Content");
                if (sub != null)
                {
                    root = sub;
                    found = await ProbeCount(root);
                }
            }

            PortContentManager.SaveFolder(root);
            PortContentManager.SetAssetCount(found);
            return found > 0;
        }

        private static async Task<int> ProbeCount(StorageFolder folder)
        {
            if (folder == null)
                return 0;
            var count = 0;
            foreach (var relative in ProbeFiles)
            {
                var file = await TryFile(folder, relative);
                if (file != null)
                    count++;
            }
            return count;
        }

        private static async Task<StorageFile> TryFile(StorageFolder folder, string relative)
        {
            try { return await folder.GetFileAsync(relative); }
            catch (Exception) { return null; }
        }

        private static async Task<StorageFolder> TryFolder(StorageFolder folder, string name)
        {
            try { return await folder.GetFolderAsync(name); }
            catch (Exception) { return null; }
        }
    }
}
