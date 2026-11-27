using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;
using StardewValley;

namespace LumiaValley.Pages
{
    public sealed partial class GamePage : Page
    {
        private readonly Game1 _game;

        public GamePage()
        {
            InitializeComponent();
            // Tenta iniciar o Game1 ORIGINAL do Stardew 1.5.6.
            _game = XamlGame<Game1>.Create(string.Empty, Window.Current.CoreWindow, SwapPanel);

            DisplayInformation.AutoRotationPreferences =
                DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;
        }
    }
}
