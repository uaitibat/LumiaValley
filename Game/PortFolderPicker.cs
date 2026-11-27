using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace LumiaValley.Game
{
    /// <summary>
    /// Abre o seletor de pasta, CONTA os XNB encontrados e so aceita a
    /// pasta se pelo menos um asset-chave existir. O resultado fica em
    /// cache (LocalSettings) para a home mostrar quantos foram carregados.
    /// </summary>
    public static class PortFolderPicker
    {
        private static readonly string[] KeyAssets =
        {
            "Maps/FarmHouse.xnb",
            "Data/Furniture.xnb",
            "LooseSprites/Cursors.pt-BR.xnb"
        };

        public static async Task<bool> PickAsync()
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List
            };
            picker.FileTypeFilter.Add("*");

            StorageFolder folder = null;
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                folder = await picker.PickSingleFolderAsync();
            });

            if (folder == null)
            {
                PortContentManager.SetAssetCount(0);
                return false;
            }

            // O usuario pode ter escolhido o Content/ ou a pasta pai.
            var root = folder;
            try
            {
                var sub = await folder.GetFolderAsync("Content");
                if (sub != null)
                    root = sub;
            }
            catch (Exception) { }

            var count = await CountXnb(root);
            var valid = count > 0 && await HasKeyAssets(root);
            PortContentManager.SaveFolder(folder);
            PortContentManager.SetAssetCount(valid ? count : 0);
            return valid;
        }

        private static async Task<int> CountXnb(StorageFolder folder)
        {
            try
            {
                var options = new QueryOptions(CommonFolderQuery.DefaultQuery);
                options.FileTypeFilter.Add(".xnb");
                var query = folder.CreateFileQueryWithOptions(options);
                var files = await query.GetFilesAsync();
                return files.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Contar XNB: " + ex.Message);
                return 0;
            }
        }

        private static async Task<bool> HasKeyAssets(StorageFolder folder)
        {
            foreach (var relative in KeyAssets)
            {
                try
                {
                    var windowsPath = relative.Replace('/', '\\');
                    if (await folder.GetFileAsync(windowsPath) != null)
                        return true; // pelo menos um asset-chave basta
                }
                catch (Exception) { }
            }
            return false;
        }
    }
}
