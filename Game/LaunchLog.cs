using System;
using System.IO;
using System.Text;
using Windows.Storage;

namespace LumiaValley.Game
{
    /// <summary>
    /// Log persistido em arquivo e mostrado no launcher. Usa System.IO direto
    /// no caminho do LocalFolder (sincrono e confiavel no UWP), para que o
    /// log da sessao que fechou apareça na proxima abertura.
    /// </summary>
    public static class LaunchLog
    {
        private static readonly StringBuilder Buf = new StringBuilder();
        private static readonly string Path;

        static LaunchLog()
        {
            try
            {
                Path = System.IO.Path.Combine(
                    ApplicationData.Current.LocalFolder.Path, "launch.log");
                if (File.Exists(Path))
                {
                    Buf.AppendLine("=== sessao anterior ===");
                    Buf.Append(File.ReadAllText(Path));
                    Buf.AppendLine("=== nova sessao ===");
                }
            }
            catch (Exception)
            {
            }
        }

        public static string Text { get { return Buf.ToString(); } }
        public static event Action Changed;

        public static void Write(string line)
        {
            var text = DateTime.Now.ToString("HH:mm:ss") + " " + line + Environment.NewLine;
            Buf.Append(text);
            System.Diagnostics.Debug.WriteLine(text);
            Flush(text);
            Changed?.Invoke();
        }

        private static void Flush(string text)
        {
            if (string.IsNullOrEmpty(Path)) return;
            try
            {
                using (var stream = new FileStream(Path, FileMode.Append,
                    FileAccess.Write, FileShare.ReadWrite))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(text);
                    writer.Flush();
                }
            }
            catch (Exception) { }
        }
    }
}
