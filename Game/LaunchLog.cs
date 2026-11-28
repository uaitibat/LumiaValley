using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace LumiaValley.Game
{
    /// <summary>
    /// Log em memoria + arquivo no LocalFolder. O launcher mostra na tela
    /// e o arquivo fica disponivel para exportacao pelo FileSavePicker.
    /// </summary>
    public static class LaunchLog
    {
        private static readonly StringBuilder Buf = new StringBuilder();
        public static event Action Changed;

        public static string Text { get { return Buf.ToString(); } }

        public static void Write(string line)
        {
            var text = DateTime.Now.ToString("HH:mm:ss") + " " + line;
            Buf.AppendLine(text);
            System.Diagnostics.Debug.WriteLine(text);
            try
            {
                var file = ApplicationData.Current.LocalFolder
                    .CreateFileAsync("launch.log", CreationCollisionOption.OpenIfExists)
                    .AsTask().GetAwaiter().GetResult();
                FileIO.AppendTextAsync(file, text + "\n")
                    .AsTask().GetAwaiter().GetResult();
            }
            catch (Exception) { }
            Changed?.Invoke();
        }
    }
}
