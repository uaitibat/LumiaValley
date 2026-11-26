namespace LumiaValley.Game
{
    /// <summary>
    /// Preferencias locais do port. Persistencia completa vem depois; por ora
    /// bastam os dois ajustes que afetam diretamente o jogo na Fase 2.
    /// </summary>
    public static class Settings
    {
        public static bool SoundEnabled = true;

        /// <summary>
        /// Qualidade da cena: 640x360 (padrao, leve na Adreno 305) ou 1280x720.
        /// </summary>
        public static bool HighQuality = false;

        public static int SceneWidth
        {
            get { return HighQuality ? 1280 : 640; }
        }

        public static int SceneHeight
        {
            get { return HighQuality ? 720 : 360; }
        }
    }
}
