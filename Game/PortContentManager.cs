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

        protected override Stream OpenStream(string assetName)
        {
            ResolveFolder();
            if (_folder != null)
            {
                try
                {
                    // StorageFolder.GetFileAsync e o unico modo de ler a pasta
                    // concedida pelo picker no sandbox UWP.
                    var relative = assetName.Replace('/', '\\') + ".xnb";
                    var file = _folder.GetFileAsync(relative).AsTask()
                        .GetAwaiter().GetResult();
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
