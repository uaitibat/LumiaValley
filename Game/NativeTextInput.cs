using System;

namespace LumiaValley.Game
{
    /// <summary>
    /// Ponte entre a logica do jogo (que nao referencia XAML) e o TextBox
    /// invisivel da GamePage. Focar esse TextBox faz o Windows 10 Mobile
    /// abrir o teclado virtual nativo (SIP).
    /// Retorna false se a pagina nao conseguiu abrir a caixa, para que o
    /// jogo use o teclado desenhado como fallback.
    /// </summary>
    public static class NativeTextInput
    {
        public static Func<string, Action<string>, Action, bool> Show;

        public static Action Hide;

        public static bool IsOpen { get; set; }

        public static bool Available
        {
            get { return Show != null; }
        }
    }
}
