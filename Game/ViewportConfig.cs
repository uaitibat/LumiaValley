using Microsoft.Xna.Framework;

namespace LumiaValley.Game
{
    /// <summary>
    /// Configuracao de resolucao do port.
    ///
    /// Lumia 640 em paisagem: 1280x720 pixels fisicos.
    /// Stardew Valley 1.5.6: logica em coordenadas virtuais 1280x720.
    ///
    /// Renderiza a metade (640x360) porque a Adreno 305 / 1 GB RAM nao
    /// sustenta 1280x720. O compositor UWP escala o backbuffer em 2x.
    ///
    /// Todo codigo portado desenha em coordenadas virtuais; o SpriteBatch
    /// aplica a escala e o touch e convertido de volta antes do uso.
    /// </summary>
    public static class ViewportConfig
    {
        public const int VirtualWidth = 1280;
        public const int VirtualHeight = 720;

        public const int BackBufferWidth = 640;
        public const int BackBufferHeight = 360;

        public const float Scale = (float)BackBufferWidth / VirtualWidth;

        public static Vector2 ToVirtual(Vector2 backBufferPoint)
        {
            return backBufferPoint / Scale;
        }
    }
}
