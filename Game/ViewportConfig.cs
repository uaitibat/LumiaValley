using Microsoft.Xna.Framework;

namespace LumiaValley.Game
{
    /// <summary>
    /// Configuracao de resolucao do port. Valores virtuais acompanham a
    /// orientacao: o launcher e vertical (720x1280) e o jogo e horizontal
    /// (1280x720), sempre na mesma escala 2:1 para a cena.
    /// </summary>
    public static class ViewportConfig
    {
        public static int VirtualWidth { get; private set; } = 1280;
        public static int VirtualHeight { get; private set; } = 720;

        public static int SceneWidth { get; private set; } = 640;
        public static int SceneHeight { get; private set; } = 360;

        public static bool IsPortrait { get; private set; }

        public static void SetPortrait(bool portrait)
        {
            IsPortrait = portrait;
            VirtualWidth = portrait ? 720 : 1280;
            VirtualHeight = portrait ? 1280 : 720;
            SceneWidth = VirtualWidth / 2;
            SceneHeight = VirtualHeight / 2;
        }

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
