using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;

namespace LumiaValley.Pages
{
    /// <summary>
    /// Host do port 1:1: GameRunner e o Game do original da 1.5.6. Ele cria
    /// o Game1, chama Instance_Initialize/Instance_LoadContent e roda o jogo
    /// original. Nenhuma tela do port incremental e usada aqui.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private readonly StardewValley.GameRunner _game;

        public GamePage()
        {
            InitializeComponent();
            _game = XamlGame<StardewValley.GameRunner>.Create(
                string.Empty, Window.Current.CoreWindow, SwapPanel);
            DisplayInformation.AutoRotationPreferences =
                DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;
        }
    }
}
