using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Windows.Storage;

namespace LumiaValley.Game
{
    /// <summary>
    /// ContentManager "tipo port": le os XNB da StorageFolder escolhida pelo
    /// usuario (cartao SD). A restauracao e LAZY e usa APIs assincronas do
    /// StorageFolder — File.Exists/File.OpenRead NAO enxergam o caminho
    /// concedido pelo FutureAccessList no sandbox UWP.
    /// </summary>
    public sealed class PortContentManager : ContentManager
    {
        private const string TokenKey = "LumiaValley.GameFolderToken";
        private StorageFolder _folder;
        private bool _folderResolved;

        public PortContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
        }

        public bool HasExternalFolder
        {
            get { ResolveFolder(); return _folder != null; }
        }

        // Cache persistido da ultima validacao da pasta (quantos XNB achou).
        private const string CountKey = "LumiaValley.AssetCount";

        public static int GetAssetCount()
        {
            try
            {
                object value;
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values
                    .TryGetValue(CountKey, out value))
                    return (int)value;
            }
            catch (Exception) { }
            return 0;
        }

        public static void SetAssetCount(int count)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[CountKey] = count;
        }

        private void ResolveFolder()
        {
            if (_folderResolved)
                return;
            _folderResolved = true;
            try
            {
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                object value;
                if (!settings.Values.TryGetValue(TokenKey, out value))
                    return;
                var token = value as string;
                if (string.IsNullOrEmpty(token))
                    return;
                _folder = Windows.Storage.AccessCache.StorageApplicationPermissions
                    .FutureAccessList.GetFolderAsync(token).AsTask().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _folder = null;
            }
        }

        public static void SaveFolder(StorageFolder folder)
        {
            var token = Windows.Storage.AccessCache.StorageApplicationPermissions
                .FutureAccessList.Add(folder);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[TokenKey] = token;
        }

        private static StorageFile TryGetFile(StorageFolder folder, string relative)
        {
            try
            {
                return folder.GetFileAsync(relative).AsTask().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static StorageFolder TryGetFolder(StorageFolder folder, string name)
        {
            try
            {
                return folder.GetFolderAsync(name).AsTask().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override Stream OpenStream(string assetName)
        {
            // 1) LocalFolder do app (copiado pelo picker) — sempre legivel.
            try
            {
                var local = Windows.Storage.ApplicationData.Current.LocalFolder;
                var localContent = local.GetFolderAsync("Content").AsTask()
                    .GetAwaiter().GetResult();
                if (localContent != null)
                {
                    var relative = assetName.Replace('/', '\\') + ".xnb";
                    var localFile = localContent.GetFileAsync(relative).AsTask()
                        .GetAwaiter().GetResult();
                    if (localFile != null)
                        return localFile.OpenStreamForReadAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception) { }

            ResolveFolder();
            if (_folder != null)
            {
                try
                {
                    // O usuario pode ter escolhido o proprio Content/ ou a
                    // pasta PAI dele. Tentamos as duas formas.
                    var relative = assetName.Replace('/', '\\') + ".xnb";
                    var file = TryGetFile(_folder, relative);
                    if (file == null)
                    {
                        var contentFolder = TryGetFolder(_folder, "Content");
                        if (contentFolder != null)
                            file = TryGetFile(contentFolder, relative);
                    }
                    if (file != null)
                        return file.OpenStreamForReadAsync().GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                    // Asset ausente na pasta: cai para o embutido.
                }
            }
            return base.OpenStream(assetName);
        }
    }
}
