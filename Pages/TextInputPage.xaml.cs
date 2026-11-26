using System;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LumiaValley.Game;

namespace LumiaValley.Pages
{
    /// <summary>Pagina separada apenas para digitar, fora do SwapChainPanel.</summary>
    public sealed partial class TextInputPage : Page
    {
        private Action<string> _changed;
        private Action _closed;

        public TextInputPage()
        {
            InitializeComponent();
            Entry.KeyDown += (sender, args) =>
            {
                if (args.Key == VirtualKey.Enter)
                    Confirm();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var data = e.Parameter as TextInputRequest;
            if (data == null)
                return;

            _changed = data.Changed;
            _closed = data.Closed;
            Entry.Text = data.Current ?? string.Empty;
            Entry.Focus(FocusState.Programmatic);
            InputPane.GetForCurrentView().TryShow();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Confirm();
        }

        private void Confirm()
        {
            try { InputPane.GetForCurrentView().TryHide(); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }

            var changed = _changed;
            var closed = _closed;
            _changed = null;
            _closed = null;

            if (Frame.CanGoBack)
                Frame.GoBack();

            if (changed != null)
                changed(Entry.Text);
            if (closed != null)
                closed();
        }
    }

    /// <summary>Dados passados para a pagina de texto.</summary>
    public sealed class TextInputRequest
    {
        public string Current;
        public Action<string> Changed;
        public Action Closed;
    }
}
