using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LumiaValley
{
    sealed partial class App : Application
    {
        public App()
        {
            // Log MINIMO no construtor: sem isso nao sabemos se o App inicia.
            Mark("App ctor");
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Mark("OnLaunched");
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
                {
                    Mark("Navegando para GamePage");
                    rootFrame.Navigate(typeof(Pages.GamePage), e.Arguments);
                    Mark("Navegou");
                }
                Window.Current.Activate();
                Mark("Ativado");
            }
        }

        internal static void Mark(string text)
        {
            try
            {
                var file = Windows.Storage.KnownFolders.DocumentsLibrary
                    .CreateFileAsync("LumiaValley-crash.log",
                        Windows.Storage.CreationCollisionOption.OpenIfExists)
                    .AsTask().GetAwaiter().GetResult();
                Windows.Storage.FileIO.AppendTextAsync(file,
                    DateTime.Now.ToString("o") + " " + text + "\n")
                    .AsTask().GetAwaiter().GetResult();
            }
            catch (Exception) { }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Mark("NAV FALHOU: " + e.Exception);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
