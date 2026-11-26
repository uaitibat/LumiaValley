using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            // O MonoGame redefine AutoRotationPreferences em SupportedOrientations.
            // Reafirma depois da criacao para garantir paisagem tambem no SwapChainPanel.
            DisplayInformation.AutoRotationPreferences =
                DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;
        }
    }
}
