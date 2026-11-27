using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;

namespace LumiaValley.Pages
{
    public sealed partial class GamePage : Page
    {
        private readonly LumiaValley.Game.Game1 _game;

        public GamePage()
        {
            InitializeComponent();
            // O host continua sendo o Game1 do port incremental; o codigo original
            // e compilado junto para permitir trocar sistemas por classes 1:1.
            _game = XamlGame<LumiaValley.Game.Game1>.Create(string.Empty, Window.Current.CoreWindow, SwapPanel);

            // O launcher e vertical; o jogo troca para paisagem ao iniciar
            // (Game1.ChangeScreen -> SupportedOrientations).
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }
    }
}
