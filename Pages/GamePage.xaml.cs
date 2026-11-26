using System;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LumiaValley.Game;
using MonoGame.Framework;

namespace LumiaValley.Pages
{
    public sealed partial class GamePage : Page
    {
        private readonly Game1 _game;

        public GamePage()
        {
            InitializeComponent();
            _game = XamlGame<Game1>.Create(string.Empty, Window.Current.CoreWindow, SwapPanel);

            DisplayInformation.AutoRotationPreferences =
                DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;

            NativeTextInput.Show = (text, changed, closed) =>
            {
                try
                {
                    // Pausa o desenho antes de trocar a pagina: o SwapChainPanel
                    // sai da arvore visual enquanto a pagina de texto esta aberta.
                    Game1.InputModal = true;
                    NativeTextInput.IsOpen = true;
                    Frame.Navigate(typeof(TextInputPage),
                        new TextInputRequest { Current = text, Changed = changed, Closed = closed });
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("NativeTextInput.Show: " + ex.Message);
                    Game1.InputModal = false;
                    NativeTextInput.IsOpen = false;
                    return false;
                }
            };

            NativeTextInput.Hide = () =>
            {
                // A pagina de texto se fecha sozinha pelo botao OK/Enter.
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Game1.InputModal = false;
            NativeTextInput.IsOpen = false;
        }
    }
}
