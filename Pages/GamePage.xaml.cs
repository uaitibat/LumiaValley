using System;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LumiaValley.Game;
using MonoGame.Framework;

namespace LumiaValley.Pages
{
    public sealed partial class GamePage : Page
    {
        private readonly Game1 _game;
        private Action<string> _onTextChanged;
        private Action _onClosed;

        public GamePage()
        {
            InitializeComponent();
            _game = XamlGame<Game1>.Create(string.Empty, Window.Current.CoreWindow, SwapPanel);

            // O MonoGame redefine AutoRotationPreferences em SupportedOrientations.
            // Reafirma depois da criacao para garantir paisagem tambem no SwapChainPanel.
            DisplayInformation.AutoRotationPreferences =
                DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;

            NativeInput.TextChanged += (sender, args) =>
            {
                if (NativeTextInput.IsOpen && _onTextChanged != null)
                    _onTextChanged(NativeInput.Text);
            };

            // Enter fecha o teclado e devolve o foco ao jogo.
            NativeInput.KeyDown += (sender, args) =>
            {
                if (args.Key == VirtualKey.Enter)
                    CloseInput();
            };

            NativeTextInput.Show = (text, changed, closed) =>
            {
                _onTextChanged = changed;
                _onClosed = closed;
                NativeInput.Text = text ?? string.Empty;
                NativeTextInput.IsOpen = true;
                // Foco no proximo ciclo do dispatcher: o MonoGame pode estar
                // processando o toque que abriu o campo neste exato instante.
                NativeInput.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => NativeInput.Focus(FocusState.Programmatic));
            };

            NativeTextInput.Hide = CloseInput;
        }

        private void CloseInput()
        {
            if (!NativeTextInput.IsOpen)
                return;

            NativeTextInput.IsOpen = false;
            NativeInput.Focus(FocusState.Unfocused);

            var changed = _onTextChanged;
            var closed = _onClosed;
            _onTextChanged = null;
            _onClosed = null;

            if (changed != null)
                changed(NativeInput.Text);
            if (closed != null)
                closed();
        }
    }
}
