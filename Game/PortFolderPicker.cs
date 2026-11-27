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

            if (found == 0)
            {
                PortContentManager.SetAssetCount(0);
                return false;
            }

            // Copia os XNB para o LocalFolder do app. Leitura em cartao SD
            // removivel pode falhar mesmo com FutureAccessList; no LocalFolder
            // o UWP SEMPRE consegue ler.
            var copied = await CopyToLocalFolder(root);
            PortContentManager.SaveFolder(root);
            PortContentManager.SetAssetCount(copied > 0 ? copied : found);
            return copied > 0 || found > 0;
        }

        /// <summary>Copia a arvore de XNB para LocalFolder/Content do app.</summary>
        private static async Task<int> CopyToLocalFolder(StorageFolder source)
        {
            var count = 0;
            try
            {
                var local = Windows.Storage.ApplicationData.Current.LocalFolder;
                var target = await local.CreateFolderAsync("Content",
                    CreationCollisionOption.OpenIfExists);

                var files = await source.GetFilesAsync();
                foreach (var file in files)
                {
                    if (!file.Name.EndsWith(".xnb", StringComparison.OrdinalIgnoreCase))
                        continue;
                    var copy = await file.CopyAsync(target, file.Name,
                        NameCollisionOption.ReplaceExisting);
                    if (copy != null) count++;
                }

                var subfolders = await source.GetFoldersAsync();
                foreach (var sub in subfolders)
                    count += await CopyFolderRecursive(sub, target);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Copiar XNB: " + ex.Message);
            }
            return count;
        }

        private static async Task<int> CopyFolderRecursive(StorageFolder source, StorageFolder parent)
        {
            var count = 0;
            try
            {
                var target = await parent.CreateFolderAsync(source.Name,
                    CreationCollisionOption.OpenIfExists);
                foreach (var file in await source.GetFilesAsync())
                {
                    if (!file.Name.EndsWith(".xnb", StringComparison.OrdinalIgnoreCase))
                        continue;
                    await file.CopyAsync(target, file.Name,
                        NameCollisionOption.ReplaceExisting);
                    count++;
                }
                foreach (var sub in await source.GetFoldersAsync())
                    count += await CopyFolderRecursive(sub, target);
            }
            catch (Exception) { }
            return count;
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
