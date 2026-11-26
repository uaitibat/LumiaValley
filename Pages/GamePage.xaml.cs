using System;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.ViewManagement;
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
                try
                {
                    if (NativeTextInput.IsOpen && _onTextChanged != null)
                        _onTextChanged(NativeInput.Text);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("NativeInput.TextChanged: " + ex.Message);
                }
            };

            NativeInput.KeyDown += (sender, args) =>
            {
                try
                {
                    if (args.Key == VirtualKey.Enter)
                        CloseInput();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("NativeInput.KeyDown: " + ex.Message);
                }
            };

            NativeTextInput.Show = (text, changed, closed) =>
            {
                try
                {
                    _onTextChanged = changed;
                    _onClosed = closed;
                    NativeInput.Text = text ?? string.Empty;
                    NativeInput.Visibility = Visibility.Visible;
                    NativeTextInput.IsOpen = true;
                    NativeInput.Focus(FocusState.Programmatic);

                    // No Windows 10 Mobile o SIP asso nao sobe so com o foco
                    // dentro de um SwapChainPanel: forcamos pelo InputPane.
                    InputPane.GetForCurrentView().TryShow();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("NativeTextInput.Show: " + ex.Message);
                    NativeTextInput.IsOpen = false;
                    NativeInput.Visibility = Visibility.Collapsed;
                    _onTextChanged = null;
                    _onClosed = null;
                    return false;
                }
            };

            NativeTextInput.Hide = () =>
            {
                try { CloseInput(); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("NativeTextInput.Hide: " + ex.Message); }
            };
        }

        private void CloseInput()
        {
            if (!NativeTextInput.IsOpen)
                return;

            NativeTextInput.IsOpen = false;

            try { InputPane.GetForCurrentView().TryHide(); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("InputPane.TryHide: " + ex.Message); }

            NativeInput.Visibility = Visibility.Collapsed;

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
