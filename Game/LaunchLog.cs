using System;
using System.Text;

namespace LumiaValley.Game
{
    /// <summary>
    /// Log em memoria, SEM I/O no construtor estatico (isso fazia o app
    /// fechar na hora no boot). O log da sessao anterior e carregado
    /// explicitamente pelo LauncherPage, depois que a tela ja esta up.
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
            // Salva imediatamente (sem timer: o timer de 2s derrubava o app).
            FlushLine(DateTime.Now.ToString("HH:mm:ss") + " " + line);
            try { Changed?.Invoke(); } catch (Exception) { }
        }

        private static void FlushLine(string text)
        {
            try
            {
                var path = System.IO.Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                    "launch.log");
                System.IO.File.AppendAllText(path, text + "\n");
            }
            catch (Exception) { }
        }

        /// <summary>Chamado pelo LauncherPage DEPOIS da tela existir.</summary>
        public static void LoadPreviousSession()
        {
            try
            {
                var path = System.IO.Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                    "launch.log");
                if (System.IO.File.Exists(path))
                {
                    var text = System.IO.File.ReadAllText(path);
                    lock (Buf)
                    {
                        Buf.Insert(0, "=== sessao anterior ===\n" + text +
                            "\n=== nova sessao ===\n");
                    }
                    Write("Log da sessao anterior carregado.");
                }
            }
            catch (Exception ex)
            {
                Write("Nao foi possivel carregar log anterior: " + ex.Message);
            }
        }

        public static void FlushToDisk()
        {
            try
            {
                var path = System.IO.Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                    "launch.log");
                System.IO.File.WriteAllText(path, Text);
            }
            catch (Exception) { }
        }
    }
}
