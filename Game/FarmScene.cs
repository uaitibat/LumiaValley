using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game
{
    /// <summary>
    /// Primeira cena jogavel do port. Nao e o Game1 descompilado: apenas
    /// usa sprites reais da 1.5.6 sobre um campo gerado proceduralmente.
    /// Toda posicao e em coordenadas virtuais 1280x720.
    /// </summary>
    public sealed class FarmScene
    {
        private static readonly Color GrassA = new Color(74, 132, 62);
        private static readonly Color GrassB = new Color(82, 142, 66);
        private static readonly Color PathColor = new Color(150, 128, 84);

        private Texture2D pixel;
        private Texture2D farmer;
        private Texture2D grass;
        private Rectangle farmerFrame;

        private Vector2 player;
        private Vector2 target;
        private bool hasTarget;

        public void Load(ContentManager content, GraphicsDevice device)
        {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] { Color.White });

            farmer = content.Load<Texture2D>("Characters/Farmer/farmer_base");
            grass = content.Load<Texture2D>("TerrainFeatures/grass");
            farmerFrame = new Rectangle(0, 0, 16, 32);

            Reset();
        }

        public void Reset()
        {
            player = new Vector2(
                ViewportConfig.VirtualWidth / 2f,
                ViewportConfig.VirtualHeight / 2f);
            hasTarget = false;
        }

        public void OnTap(Vector2 virtualPoint)
        {
            target = virtualPoint;
            hasTarget = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!hasTarget)
                return;

            var delta = target - player;
            var distance = delta.Length();
            const float speed = 260f; // pixels virtuais por segundo
            var step = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (distance <= step)
            {
                player = target;
                hasTarget = false;
            }
            else
            {
                player += delta / distance * step;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            const int tile = 64; // 16 pixels de sprite * escala 4
            var cols = ViewportConfig.VirtualWidth / tile;
            var rows = ViewportConfig.VirtualHeight / tile;

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    var center = (x == cols / 2 && y == rows / 2);
                    var color = center ? PathColor : ((x + y) % 2 == 0 ? GrassA : GrassB);
                    spriteBatch.Draw(
                        pixel,
                        new Rectangle(x * tile, y * tile, tile, tile),
                        color);

                    if (!center && x % 3 == 0 && y % 2 == 0 && grass != null && grass.Width > 16)
                    {
                        spriteBatch.Draw(
                            grass,
                            new Rectangle(x * tile + 8, y * tile + 24, 48, 40),
                            new Rectangle(0, 0, 16, 16),
                            Color.White);
                    }
                }
            }

            if (hasTarget)
            {
                spriteBatch.Draw(
                    pixel,
                    new Rectangle((int)target.X - 6, (int)target.Y - 6, 12, 12),
                    Color.Red);
            }

            // Sprite do fazendeiro: 16x32 * 4 = 64x128 no espaco virtual.
            spriteBatch.Draw(
                farmer,
                new Rectangle((int)player.X - 32, (int)player.Y - 112, 64, 128),
                farmerFrame,
                Color.White);
        }
    }
}
