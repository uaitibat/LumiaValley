using System;
using System.IO;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;

namespace LumiaValley.Pages
{
    public sealed partial class GamePage : Page
    {
        private readonly StardewValley.GameRunner _game;

        public GamePage()
        {
            InitializeComponent();
            try
            {
                Log("Criando GameRunner...");
                _game = XamlGame<StardewValley.GameRunner>.Create(
                    string.Empty, Window.Current.CoreWindow, SwapPanel);
                Log("GameRunner criado.");
            }
            catch (Exception ex)
            {
                Log("ERRO: " + ex.ToString());
                throw;
            }
            DisplayInformation.AutoRotationPreferences =
                DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;
            Log("AutoRotation aplicada.");
        }

        private static void Log(string text)
        {
            try
            {
                var file = KnownFolders.DocumentsLibrary
                    .CreateFileAsync("LumiaValley-crash.log", CreationCollisionOption.OpenIfExists)
                    .AsTask().GetAwaiter().GetResult();
                FileIO.AppendTextAsync(file, DateTime.Now.ToString("o") + " " + text + "\n")
                    .AsTask().GetAwaiter().GetResult();
            }
            catch (Exception) { }
        }
    }
}
