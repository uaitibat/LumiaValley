using System;

namespace LumiaValley.Game
{
    /// <summary>
    /// Ponte entre a logica do jogo (que nao referencia XAML) e o TextBox
    /// invisivel da GamePage. Focar esse TextBox faz o Windows 10 Mobile
    /// abrir o teclado virtual nativo (SIP).
    /// </summary>
    public static class NativeTextInput
    {
        /// <summary>Texto atual, callback a cada alteracao e ao fechar.</summary>
        public static Action<string, Action<string>, Action> Show;

        public static Action Hide;

        public static bool IsOpen { get; set; }

        public static bool Available
        {
            get { return Show != null; }
        }
    }
}
