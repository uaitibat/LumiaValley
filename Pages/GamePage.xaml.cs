using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;

namespace LumiaValley.Pages
{
    public sealed partial class GamePage : Page
    {
        // true = host ORIGINAL do Stardew 1.5.6 (GameRunner -> Game1).
        // false = host incremental (LumiaValley.Game.Game1).
        private const bool UseOriginalGame = true;

        private readonly object _game;

        public GamePage()
        {
            InitializeComponent();

            if (UseOriginalGame)
            {
                // Original: GameRunner e o Microsoft.Xna.Framework.Game que
                // cria Game1, chama Instance_Initialize/Instance_LoadContent.
                _game = XamlGame<StardewValley.GameRunner>.Create(
                    string.Empty, Window.Current.CoreWindow, SwapPanel);
                DisplayInformation.AutoRotationPreferences =
                    DisplayOrientations.Landscape;
            }
            else
            {
                _game = XamlGame<LumiaValley.Game.Game1>.Create(
                    string.Empty, Window.Current.CoreWindow, SwapPanel);
                DisplayInformation.AutoRotationPreferences =
                    DisplayOrientations.Portrait;
            }
        }
    }
}
