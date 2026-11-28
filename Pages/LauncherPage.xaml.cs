using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LumiaValley.Game;

namespace LumiaValley.Pages
{
    public sealed partial class LauncherPage : Page
    {
        public LauncherPage()
        {
            InitializeComponent();
            LaunchLog.Changed += Refresh;
            LaunchLog.LoadPreviousSession();
            Refresh();
            LaunchLog.Write("Launcher pronto.");
            LaunchLog.Write("Appx: " + Windows.ApplicationModel.Package.Current.Id.Name +
                " " + Windows.ApplicationModel.Package.Current.Id.Version);

            // Log salvo a cada Write (sem timer — o timer derrubava o app).
        }

        private void Refresh()
        {
            if (LogText == null) return;
            LogText.Text = LaunchLog.Text;
            // Scroll so ao final SE o ScrollViewer ja estiver pronto. No
            // construtor da pagina ele ainda e null e derrubava o app.
            try
            {
                if (LogScroll != null && LogScroll.ScrollableHeight > 0)
                    LogScroll.ChangeView(null, LogScroll.ScrollableHeight, null, false);
            }
            catch (Exception) { }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            LaunchLog.Clear();
            Refresh();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            LaunchLog.Write("Navegando para GamePage (GameRunner)...");
            Frame.Navigate(typeof(GamePage));
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new FileSavePicker
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    SuggestedFileName = "lumia-valley-launch.log"
                };
                picker.FileTypeChoices.Add("Log", new System.Collections.Generic.List<string> { ".log" });
                var file = await picker.PickSaveFileAsync();
                if (file != null)
                    await FileIO.WriteTextAsync(file, LaunchLog.Text);
            }
            catch (Exception ex)
            {
                LaunchLog.Write("Export falhou: " + ex.Message);
            }
        }
    }
}
