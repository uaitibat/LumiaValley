using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Base das telas de menu da Fase 2. Usa os sprites de caixa de dialogo da
    /// propria 1.5.6 para nao inventar uma identidade visual nueva.
    /// </summary>
    public abstract class MenuScreen : IScreen
    {
        protected static readonly Rectangle BackRect = new Rectangle(40, 636, 220, 56);

        protected Texture2D pixel;
        protected Texture2D panel;
        protected Texture2D textBox;
        protected SpriteFont font;

        public virtual void Load(ContentManager content, GraphicsDevice device)
        {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] { Color.White });
            panel = LoadTexture(content, "LooseSprites/DialogBoxGreen");
            textBox = LoadTexture(content, "LooseSprites/textBox");
            font = LoadFont(content, "Fonts/SmallFont.pt-BR", "Fonts/SmallFont");
        }

        public abstract IScreen Update(GameTime gameTime, Vector2? tap);

        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual Matrix Transform
        {
            get { return Matrix.Identity; }
        }

        public virtual Vector2 FromVirtual(Vector2 point)
        {
            return point;
        }

        protected static bool Contains(Rectangle r, Vector2 p)
        {
            return r.Contains((int)p.X, (int)p.Y);
        }

        protected bool BackTapped(Vector2? tap)
        {
            return tap.HasValue && Contains(BackRect, tap.Value);
        }

        protected IScreen Home()
        {
            // skipIntro evita replay da abertura a cada volta de um menu.
            return new TitleScreen(true);
        }

        protected void DrawPanel(SpriteBatch batch, Rectangle area, string title)
        {
            batch.Draw(pixel, new Rectangle(0, 0, 1280, 720), Color.Black * 0.78f);

            if (panel == null || panel.Width < 32 || panel.Height < 32)
            {
                batch.Draw(pixel, area, new Color(28, 58, 38));
                batch.Draw(pixel, new Rectangle(area.X, area.Y, area.Width, 4), new Color(126, 170, 92));
                batch.Draw(pixel, new Rectangle(area.X, area.Bottom - 4, area.Width, 4), new Color(126, 170, 92));
            }
            else
            {
                const int b = 16;
                DrawSlice(batch, area, b,
                    new Rectangle(0, 0, b, b), new Rectangle(b, 0, panel.Width - b * 2, b),
                    new Rectangle(panel.Width - b, 0, b, b),
                    new Rectangle(0, b, b, panel.Height - b * 2),
                    new Rectangle(b, b, panel.Width - b * 2, panel.Height - b * 2),
                    new Rectangle(panel.Width - b, b, b, panel.Height - b * 2),
                    new Rectangle(0, panel.Height - b, b, b),
                    new Rectangle(b, panel.Height - b, panel.Width - b * 2, b),
                    new Rectangle(panel.Width - b, panel.Height - b, b, b));
            }

            if (font != null && !string.IsNullOrEmpty(title))
            {
                batch.DrawString(font, title, new Vector2(area.X + 32, area.Y + 22),
                    new Color(255, 236, 160), 0f, Vector2.Zero, 1.6f, SpriteEffects.None, 0f);
            }
        }

        private void DrawSlice(
            SpriteBatch batch, Rectangle area, int b,
            Rectangle tl, Rectangle tc, Rectangle tr,
            Rectangle ml, Rectangle mc, Rectangle mr,
            Rectangle bl, Rectangle bc, Rectangle br)
        {
            batch.Draw(panel, new Rectangle(area.X, area.Y, b, b), tl, Color.White);
            batch.Draw(panel, new Rectangle(area.X + b, area.Y, area.Width - b * 2, b), tc, Color.White);
            batch.Draw(panel, new Rectangle(area.Right - b, area.Y, b, b), tr, Color.White);

            batch.Draw(panel, new Rectangle(area.X, area.Y + b, b, area.Height - b * 2), ml, Color.White);
            batch.Draw(panel, new Rectangle(area.X + b, area.Y + b, area.Width - b * 2, area.Height - b * 2), mc, Color.White);
            batch.Draw(panel, new Rectangle(area.Right - b, area.Y + b, b, area.Height - b * 2), mr, Color.White);

            batch.Draw(panel, new Rectangle(area.X, area.Bottom - b, b, b), bl, Color.White);
            batch.Draw(panel, new Rectangle(area.X + b, area.Bottom - b, area.Width - b * 2, b), bc, Color.White);
            batch.Draw(panel, new Rectangle(area.Right - b, area.Bottom - b, b, b), br, Color.White);
        }

        protected void DrawRow(SpriteBatch batch, Rectangle r, string text, bool selected)
        {
            if (textBox != null && textBox.Width >= 192 && textBox.Height >= 48)
            {
                batch.Draw(textBox, r, new Rectangle(0, 0, 192, 48),
                    selected ? new Color(255, 228, 150) : Color.White * 0.95f);
            }
            else
            {
                batch.Draw(pixel, r, selected ? new Color(96, 142, 72) : new Color(48, 82, 54));
            }

            if (font != null && !string.IsNullOrEmpty(text))
            {
                batch.DrawString(font, text, new Vector2(r.X + 18, r.Y + 14), Color.White);
            }
        }

        protected void DrawBack(SpriteBatch batch)
        {
            DrawRow(batch, BackRect, Localization.CS("StringsFromCSFiles:TitleMenu.cs.11739", "Voltar"), false);
        }

        private static Texture2D LoadTexture(ContentManager content, string asset)
        {
            try
            {
                return content.Load<Texture2D>(asset);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Asset indisponivel " + asset + ": " + ex.Message);
                return null;
            }
        }

        private static SpriteFont LoadFont(ContentManager content, params string[] assets)
        {
            foreach (var asset in assets)
            {
                try
                {
                    var result = content.Load<SpriteFont>(asset);
                    if (result != null)
                        return result;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Fonte indisponivel " + asset + ": " + ex.Message);
                }
            }
            return null;
        }
    }
}
