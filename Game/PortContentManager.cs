using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Windows.Storage;

namespace LumiaValley.Game
{
    /// <summary>
    /// ContentManager "tipo port": se o usuario escolheu uma pasta com os
    /// XNB do jogo, le da pasta; senao usa o Content embutido no appx.
    /// A restauracao do folder e LAZY para nao bloquear a thread de jogo
    /// no startup (GetAwaiter().GetResult() em WinRT pode travar).
    /// </summary>
    public sealed class PortContentManager : ContentManager
    {
        private const string TokenKey = "LumiaValley.GameFolderToken";
        private StorageFolder _folder;
        private bool _folderResolved;
        private string _folderPath;

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
                _folderPath = _folder == null ? null : _folder.Path;
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
            if (!string.IsNullOrEmpty(_folderPath))
            {
                try
                {
                    // Caminho direto evita GetFileAsync() assincrono na thread
                    // do jogo. O UWP concede acesso ao folder via FutureAccessList.
                    var relative = assetName.Replace('/', Path.DirectorySeparatorChar) + ".xnb";
                    var path = Path.Combine(_folderPath, relative);
                    if (File.Exists(path))
                        return File.OpenRead(path);
                }
                catch (Exception)
                {
                    // Asset ausente: cai para o embutido.
                }
            }
            return base.OpenStream(assetName);
        }
    }
}
