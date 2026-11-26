using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Tela inicial. Nesta fatia ainda e a versao minima; a Fase 1 vai
    /// substitui-la pela home fiel (ceu, nuvens, botoes e estados).
    /// </summary>
    public sealed class TitleScreen : IScreen
    {
        private static readonly Rectangle LogoSource = new Rectangle(0, 0, 400, 187);
        private static readonly Rectangle[] ButtonSources =
        {
            new Rectangle(0, 187, 74, 58),
            new Rectangle(74, 187, 74, 58),
            new Rectangle(148, 187, 74, 58),
            new Rectangle(222, 187, 74, 58)
        };

        private Texture2D pixel;
        private Texture2D dayBg;
        private Texture2D titleButtons;
        private int selected = -1;
        private bool xnbLoaded;

        public void Load(ContentManager content, GraphicsDevice device)
        {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] { Color.White });

            try
            {
                dayBg = content.Load<Texture2D>("LooseSprites/daybg");
                titleButtons = content.Load<Texture2D>("Minigames/TitleButtons");
                xnbLoaded = dayBg != null && titleButtons != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao carregar XNB da home: " + ex);
                xnbLoaded = false;
            }
        }

        private static Rectangle ButtonDest(int index)
        {
            const int width = 222;
            const int height = 174;
            const int spacing = 20;
            var total = ButtonSources.Length * width + (ButtonSources.Length - 1) * spacing;
            var x = (ViewportConfig.VirtualWidth - total) / 2 + index * (width + spacing);
            return new Rectangle(x, 470, width, height);
        }

        public IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (!tap.HasValue)
                return this;

            var x = (int)tap.Value.X;
            var y = (int)tap.Value.Y;

            for (var i = 0; i < ButtonSources.Length; i++)
            {
                if (!ButtonDest(i).Contains(x, y))
                    continue;

                selected = i;
                if (i == 0 || i == 1)
                    return new FarmScreen();

                break;
            }

            return this;
        }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle r, Color color)
        {
            const int t = 3;
            spriteBatch.Draw(pixel, new Rectangle(r.X - t, r.Y - t, r.Width + t * 2, t), color);
            spriteBatch.Draw(pixel, new Rectangle(r.X - t, r.Bottom, r.Width + t * 2, t), color);
            spriteBatch.Draw(pixel, new Rectangle(r.X - t, r.Y, t, r.Height), color);
            spriteBatch.Draw(pixel, new Rectangle(r.Right, r.Y, t, r.Height), color);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!xnbLoaded)
            {
                spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        ViewportConfig.VirtualWidth / 2 - 32,
                        ViewportConfig.VirtualHeight / 2 - 32,
                        64, 64),
                    Color.Wheat);
                return;
            }

            spriteBatch.Draw(
                dayBg,
                new Rectangle(0, 0, ViewportConfig.VirtualWidth, ViewportConfig.VirtualHeight),
                Color.White);
            spriteBatch.Draw(titleButtons, new Rectangle(240, 20, 800, 374), LogoSource, Color.White);

            for (var i = 0; i < ButtonSources.Length; i++)
            {
                var dest = ButtonDest(i);
                var tint = i == selected ? Color.White : new Color(190, 190, 190);
                spriteBatch.Draw(titleButtons, dest, ButtonSources[i], tint);

                if (i == selected)
                    DrawBorder(spriteBatch, dest, Color.Red);
            }
        }
    }
}
