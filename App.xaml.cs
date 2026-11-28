using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LumiaValley
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            // Loga qualquer excecao fatal no LocalFolder para diagnostico
            // pelo MTP (o app fecha antes de podermos ver a tela).
            UnhandledException += (sender, args) =>
            {
                try
                {
                    var text = DateTime.Now.ToString("o") + "\n" +
                               args.Exception.ToString() + "\n";
                    var file = Windows.Storage.ApplicationData.Current.LocalFolder
                        .CreateFileAsync("crash.log",
                            Windows.Storage.CreationCollisionOption.AppendToDefault)
                        .AsTask().GetAwaiter().GetResult();
                    Windows.Storage.FileIO.AppendTextAsync(file, text)
                        .AsTask().GetAwaiter().GetResult();
                }
                catch (Exception) { }
            };
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            ConfigureFullScreen();

            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                    rootFrame.Navigate(typeof(Pages.GamePage), e.Arguments);
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Trava a orientacao no nivel do sistema. O SupportedOrientations do
        /// MonoGame nao controla a rotacao de uma pagina XAML com SwapChainPanel.
        /// </summary>
        internal static void ConfigureFullScreen()
        {
            DisplayInformation.AutoRotationPreferences =
                DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;

            var view = ApplicationView.GetForCurrentView();
            view.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            if (!view.IsFullScreenMode)
                view.TryEnterFullScreenMode();

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                HideStatusBar();
                Window.Current.SizeChanged += (sender, args) => HideStatusBar();
            }
        }

        private static async void HideStatusBar()
        {
            try
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("StatusBar.HideAsync falhou: " + ex.Message);
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Falha ao carregar a pagina " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
