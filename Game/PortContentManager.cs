using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Windows.Storage;

namespace LumiaValley.Game
{
    /// <summary>
    /// ContentManager "tipo port": se o usuario escolheu uma pasta com os
    /// XNB do jogo (Data/Maps/Characters/...), le da pasta; senao usa o
    /// Content embutido no appx. OpenStream e virtual, entao basta trocar.
    /// </summary>
    public sealed class PortContentManager : ContentManager
    {
        private const string TokenKey = "LumiaValley.GameFolderToken";
        private readonly StorageFolder _folder;

        public PortContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
            _folder = TryRestoreFolder();
        }

        public bool HasExternalFolder { get { return _folder != null; } }

        private static StorageFolder TryRestoreFolder()
        {
            try
            {
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                object value;
                if (!settings.Values.TryGetValue(TokenKey, out value))
                    return null;
                var token = value as string;
                if (string.IsNullOrEmpty(token))
                    return null;
                return Windows.Storage.AccessCache.StorageApplicationPermissions
                    .FutureAccessList.GetFolderAsync(token).AsTask().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return null;
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
            if (_folder != null)
            {
                try
                {
                    var relative = assetName.Replace('/', Path.DirectorySeparatorChar) + ".xnb";
                    var file = _folder.GetFileAsync(relative).AsTask().GetAwaiter().GetResult();
                    if (file != null)
                        return file.OpenStreamForReadAsync().GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                    // Asset ausente na pasta externa: cai para o embutido.
                }
            }
            return base.OpenStream(assetName);
        }
    }
}
