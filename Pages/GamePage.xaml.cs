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

            NativeOk.Click += (sender, args) =>
            {
                try { CloseInput(); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("NativeOk: " + ex.Message); }
            };

            NativeTextInput.Show = (text, changed, closed) =>
            {
                try
                {
                    _onTextChanged = changed;
                    _onClosed = closed;
                    NativeInput.Text = text ?? string.Empty;
                    InputPopup.IsOpen = true;
                    NativeTextInput.IsOpen = true;
                    NativeInput.Focus(FocusState.Programmatic);
                    InputPane.GetForCurrentView().TryShow();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("NativeTextInput.Show: " + ex.Message);
                    Fail();
                    return false;
                }
            };

            NativeTextInput.Hide = () =>
            {
                try { CloseInput(); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("NativeTextInput.Hide: " + ex.Message); }
            };
        }

        private void Fail()
        {
            NativeTextInput.IsOpen = false;
            InputPopup.IsOpen = false;
            _onTextChanged = null;
            _onClosed = null;
        }

        private void CloseInput()
        {
            if (!NativeTextInput.IsOpen)
                return;

            NativeTextInput.IsOpen = false;

            try { InputPane.GetForCurrentView().TryHide(); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("InputPane.TryHide: " + ex.Message); }

            InputPopup.IsOpen = false;

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
