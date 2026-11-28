using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LumiaValley.Game;

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
                // SEM ISSO o UWP mata o app mesmo apos mostrar o erro.
                e.Handled = true;
                LaunchLog.Write("UNHANDLED: " + e.Exception);
                var text = "CRASH LumiaValley\n" + e.Exception;
                try
                {
                    var box = new TextBlock
                    {
                        Text = text,
                        Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(
                            Windows.UI.Colors.White),
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 14,
                        Margin = new Thickness(12)
                    };
                    var panel = new StackPanel
                    {
                        Background = new Windows.UI.Xaml.Media.SolidColorBrush(
                            Windows.UI.Colors.Black)
                    };
                    panel.Children.Add(box);
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
                    rootFrame.Navigate(typeof(Pages.LauncherPage), e.Arguments);
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            LaunchLog.Write("NAV FALHOU: " + e.Exception);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
