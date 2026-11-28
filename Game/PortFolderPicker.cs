using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace LumiaValley.Game
{
    /// <summary>
    /// Abre o seletor, lista recursivamente os arquivos da pasta escolhida
    /// (funciona em cartao SD, onde GetFileAsync com caminho relativo falha),
    /// acha a raiz do Content e copia os XNB para o LocalFolder do app.
    /// </summary>
    public static class PortFolderPicker
    {
        public static string LastDiagnostic { get; private set; } = "";

        public static async Task<bool> PickAsync()
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List
            };
            picker.FileTypeFilter.Add("*");

            // Chamar o picker DIRETO (sem RunAsync aninhado): o RunAsync fazia
            // o seletor abrir fora do contexto da pagina e retornar null.
            var picked = await picker.PickSingleFolderAsync();
            LastDiagnostic = "Seletor fechou. Abrindo pasta...";

            if (picked == null)
            {
                PortContentManager.SetAssetCount(0);
                LastDiagnostic = "Nenhuma pasta escolhida.";
                return false;
            }

            LastDiagnostic = "Pasta: " + picked.Name;
            var files = new List<StorageFile>();
            await ListRecursive(picked, files, 0);

            // Acha a raiz do Content: onde existir Mapas/FarmHouse.xnb etc.
            StorageFolder root = FindContentRoot(picked, files);
            LastDiagnostic += " | " + files.Count + " arquivos listados";

            if (root == null)
            {
                PortContentManager.SetAssetCount(0);
                LastDiagnostic += " | raiz do Content NAO encontrada";
                return false;
            }

            var xnb = 0;
            foreach (var f in files)
                if (f.Name.EndsWith(".xnb", StringComparison.OrdinalIgnoreCase))
                    xnb++;
            LastDiagnostic += " | " + xnb + " XNB";

            var copied = await CopyToLocalFolder(root);
            PortContentManager.SaveFolder(root);
            PortContentManager.SetAssetCount(copied > 0 ? copied : xnb);
            LastDiagnostic += " | " + copied + " copiados";
            return copied > 0 || xnb > 0;
        }

        private static async Task ListRecursive(StorageFolder folder, List<StorageFile> result, int depth)
        {
            if (depth > 4 || result.Count > 5000)
                return;
            try
            {
                foreach (var f in await folder.GetFilesAsync())
                    result.Add(f);
                foreach (var sub in await folder.GetFoldersAsync())
                    await ListRecursive(sub, result, depth + 1);
            }
            catch (Exception ex)
            {
                LastDiagnostic += " | erro listar: " + ex.Message;
            }
        }

        private static StorageFolder FindContentRoot(StorageFolder picked, List<StorageFile> files)
        {
            // Se a propria pasta tem Maps/Characters/Data, é a raiz.
            foreach (var f in files)
            {
                if (f.Name.Equals("FarmHouse.xnb", StringComparison.OrdinalIgnoreCase))
                    return picked;
            }
            // Senao tenta a subpasta Content.
            try
            {
                var sub = picked.GetFolderAsync("Content").AsTask().GetAwaiter().GetResult();
                if (sub != null) return sub;
            }
            catch (Exception) { }
            return null;
        }

        private static async Task<int> CopyToLocalFolder(StorageFolder source)
        {
            var count = 0;
            try
            {
                var local = Windows.Storage.ApplicationData.Current.LocalFolder;
                var target = await local.CreateFolderAsync("Content",
                    CreationCollisionOption.OpenIfExists);
                count += await CopyFolderRecursive(source, target);
            }
            catch (Exception ex)
            {
                LastDiagnostic += " | erro copiar: " + ex.Message;
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
                    await file.CopyAsync(target, file.Name, NameCollisionOption.ReplaceExisting);
                    count++;
                }
                foreach (var sub in await source.GetFoldersAsync())
                    count += await CopyFolderRecursive(sub, target);
            }
            catch (Exception) { }
            return count;
        }
    }
}
