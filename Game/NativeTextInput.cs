using System;

namespace LumiaValley.Game
{
    /// <summary>
    /// Ponte entre o jogo (sem referencia a XAML) e o Popup com TextBox da
    /// GamePage. Retorna false se a pagina nao abrir o teclado, para que o
    /// jogo use o teclado desenhado como fallback.
    /// </summary>
    public static class NativeTextInput
    {
        public static Func<string, Action<string>, Action, bool> Show;
        public static Action Hide;
        public static bool IsOpen { get; set; }
        public static bool Available { get { return Show != null; } }
    }
}
