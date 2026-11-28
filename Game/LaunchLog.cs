using System;
using System.Text;

namespace LumiaValley.Game
{
    /// <summary>
    /// Log APENAS em memoria — nenhum I/O em disco. I/O no boot fazia o
    /// app fechar na hora no Lumia (mesmo dentro de try/catch: e um crash
    /// nativo que o managed nao pega). O texto aparece no launcher.
    /// </summary>
    public static class LaunchLog
    {
        private static readonly StringBuilder Buf = new StringBuilder();
        public static event Action Changed;

        public static string Text
        {
            get
            {
                lock (Buf) return Buf.ToString();
            }
        }

        public static void Write(string line)
        {
            try
            {
                lock (Buf)
                {
                    Buf.AppendLine(DateTime.Now.ToString("HH:mm:ss") + " " + line);
                }
                System.Diagnostics.Debug.WriteLine(line);
            }
            catch (Exception) { }
            try { Changed?.Invoke(); } catch (Exception) { }
        }

        // Mantidos para compatibilidade com chamadas existentes, mas sem I/O.
        public static void LoadPreviousSession() { }
        public static void FlushToDisk() { }
        public static void Clear()
        {
            lock (Buf) Buf.Clear();
            Write("Log limpo.");
        }
    }
}
