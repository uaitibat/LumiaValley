using Microsoft.Xna.Framework;

namespace LumiaValley.Game
{
    /// <summary>
    /// Configuracao de resolucao do port.
    ///
    /// Lumia 640 em paisagem: 1280x720 pixels fisicos.
    /// Stardew Valley 1.5.6: logica em coordenadas virtuais 1280x720.
    ///
    /// A cena e desenhada em 640x360 (SceneWidth/SceneHeight) e depois
    /// escalada para o viewport real do SwapChainPanel. Nao impomos
    /// PreferredBackBuffer: no UWP o MonoGame dimensiona a swap chain pelo painel.
    /// </summary>
    public static class ViewportConfig
    {
        public const int VirtualWidth = 1280;
        public const int VirtualHeight = 720;

        public const int PreferredBackBufferWidth = 640;
        public const int PreferredBackBufferHeight = 360;

        /// <summary>Resolucao da cena desenhada antes do blit final.</summary>
        public const int SceneWidth = 640;
        public const int SceneHeight = 360;

        /// <summary>Escala do espaco virtual para o backbuffer efetivo.</summary>
        public static Vector2 GetScale(int backBufferWidth, int backBufferHeight)
        {
            return new Vector2(
                (float)backBufferWidth / VirtualWidth,
                (float)backBufferHeight / VirtualHeight);
        }

        /// <summary>Converte um ponto de touch do backbuffer para o espaco virtual.</summary>
        public static Vector2 ToVirtual(Vector2 point, int backBufferWidth, int backBufferHeight)
        {
            var scale = GetScale(backBufferWidth, backBufferHeight);
            return new Vector2(point.X / scale.X, point.Y / scale.Y);
        }
    }
}
