using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Windows.Storage;

namespace LumiaValley.Game
{
    /// <summary>
    /// ContentManager "tipo port". O Content real (515 MB) esta EMBUTIDO no
    /// appx, entao a prioridade e: appx -> LocalFolder -> pasta externa.
    /// Antes a ordem era invertida e restos de picker/previous installs
    /// faziam o jogo carregar assets errados (tela azul-clara).
    /// </summary>
    public sealed class PortContentManager : ContentManager
    {
        private const string TokenKey = "LumiaValley.GameFolderToken";
        private const string CountKey = "LumiaValley.AssetCount";
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

        public static void SaveFolder(StorageFolder folder)
        {
            var token = Windows.Storage.AccessCache.StorageApplicationPermissions
                .FutureAccessList.Add(folder);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[TokenKey] = token;
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

        protected override Stream OpenStream(string assetName)
        {
            // 1) Content EMBUTIDO no appx (sempre correto).
            try
            {
                return base.OpenStream(assetName);
            }
            catch (Exception)
            {
                // Nao estava no appx: tenta as fontes externas abaixo.
            }

            // 2) LocalFolder (copiado pelo picker, se usado).
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

            // 3) Pasta externa escolhida no picker.
            ResolveFolder();
            if (_folder != null)
            {
                try
                {
                    var relative = assetName.Replace('/', '\\') + ".xnb";
                    var file = _folder.GetFileAsync(relative).AsTask().GetAwaiter().GetResult();
                    if (file == null)
                    {
                        var contentFolder = _folder.GetFolderAsync("Content").AsTask()
                            .GetAwaiter().GetResult();
                        if (contentFolder != null)
                            file = contentFolder.GetFileAsync(relative).AsTask()
                                .GetAwaiter().GetResult();
                    }
                    if (file != null)
                        return file.OpenStreamForReadAsync().GetAwaiter().GetResult();
                }
                catch (Exception) { }
            }

            throw new FileNotFoundException("Asset nao encontrado: " + assetName);
        }
    }
}
