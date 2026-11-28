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

            // Salva o log a cada 2s para sobreviver ao fechamento do app.
            var timer = new Windows.UI.Xaml.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, e) => LaunchLog.FlushToDisk();
            timer.Start();
        }

        private void Refresh()
        {
            LogText.Text = LaunchLog.Text;
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
