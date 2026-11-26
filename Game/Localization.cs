using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace LumiaValley.Game
{
    /// <summary>
    /// Acesso as strings localizadas do 1.5.6. Os XNB de Strings sao
    /// Dictionary&lt;string,string&gt;; carregamos apenas os que a UI usa.
    /// Nenhuma chave e obrigatoria: sempre ha um fallback em portugues.
    /// </summary>
    public static class Localization
    {
        private static Dictionary<string, string> csFiles;
        private static Dictionary<string, string> ui;
        private static bool loaded;

        public static void Load(ContentManager content)
        {
            if (loaded)
                return;

            loaded = true;
            csFiles = TryLoad(content, "Strings/StringsFromCSFiles.pt-BR");
            ui = TryLoad(content, "Strings/UI.pt-BR");
        }

        private static Dictionary<string, string> TryLoad(ContentManager content, string asset)
        {
            try
            {
                return content.Load<Dictionary<string, string>>(asset)
                       ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao carregar " + asset + ": " + ex.Message);
                return new Dictionary<string, string>();
            }
        }

        /// <summary>String de StringsFromCSFiles.pt-BR, com fallback.</summary>
        public static string CS(string key, string fallback)
        {
            string value;
            if (csFiles != null && csFiles.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
                return value;
            return fallback;
        }

        /// <summary>String de UI.pt-BR, com fallback.</summary>
        public static string UIString(string key, string fallback)
        {
            string value;
            if (ui != null && ui.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
                return value;
            return fallback;
        }
    }
}
