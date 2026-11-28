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
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += (sender, e) =>
            {
                // Mostra o erro na tela para fotografia via MTP/camera.
                try
                {
                    var panel = new StackPanel
                    {
                        Background = new Windows.UI.Xaml.Media.SolidColorBrush(
                            Windows.UI.Colors.Black)
                    };
                    var text = new TextBlock
                    {
                        Text = "CRASH LumiaValley\n" + e.Exception,
                        Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(
                            Windows.UI.Colors.White),
                        TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap,
                        FontSize = 14,
                        Margin = new Windows.UI.Xaml.Thickness(12)
                    };
                    panel.Children.Add(text);
                    Window.Current.Content = panel;
                    Window.Current.Activate();
                }
                catch (Exception) { }
            };
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
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
