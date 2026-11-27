using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace LumiaValley.Game
{
    /// <summary>
    /// Abre o seletor de pasta do UWP para o usuario apontar para o
    /// "Content/" do Stardew (copiado do PC para o cartao SD). O acesso
    /// fica persistido no FutureAccessList.
    /// </summary>
    public static class PortFolderPicker
    {
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
                return false;

            PortContentManager.SaveFolder(folder);
            return true;
        }
    }
}
