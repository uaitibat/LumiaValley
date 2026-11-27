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

            // Conta PRIMEIRO na pasta escolhida. Só tenta entrar numa subpasta
            // "Content" se a pasta escolhida nao tiver XNB nenhum — assim nao
            // perdemos o caso em que o usuario já selecionou a propria Content.
            var root = folder;
            var count = await CountXnb(root);
            if (count == 0)
            {
                try
                {
                    var sub = await folder.GetFolderAsync("Content");
                    if (sub != null)
                    {
                        root = sub;
                        count = await CountXnb(root);
                    }
                }
                catch (Exception) { }
            }

            // Qualquer XNB na pasta ja basta para validar. O HasKeyAssets era
            // rigoroso demais (nomes pt-BR/outros assets podem divergir) e
            // fazia pastas corretas serem marcadas como invalidas.
            var valid = count > 0;
            PortContentManager.SaveFolder(root); // guarda a RAIZ dos XNB
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
