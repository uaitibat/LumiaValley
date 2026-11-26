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

        // Cores do original 1.5.6: texto escuro e sombra laranja de
        // Utility.drawTextWithShadow.
        protected static readonly Color TextColor = new Color(34, 17, 34);
        protected static readonly Color ShadowColor = new Color(221, 148, 84);

        protected Texture2D pixel;
        protected Texture2D menuTiles;
        protected Texture2D textBox;
        protected SpriteFont font;
        protected SpriteFont dialogueFont;

        public virtual void Load(ContentManager content, GraphicsDevice device)
        {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] { Color.White });
            menuTiles = LoadTexture(content, "Maps/MenuTiles");
            textBox = LoadTexture(content, "LooseSprites/textBox");
            font = LoadFont(content, "Fonts/SmallFont.pt-BR", "Fonts/SmallFont");
            dialogueFont = LoadFont(content, "Fonts/SpriteFont1.pt-BR", "Fonts/SpriteFont1");
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

        /// <summary>
        /// Equivale a Game1.drawDialogueBox: o miolo comeca 28px dentro e as
        /// bordas sao desenhadas sem offset. A area transparente das bordas e
        /// preenchida pelo miolo, sem deixar o fundo aparecer.
        /// </summary>
        protected void DrawPanel(SpriteBatch batch, Rectangle area, string title)
        {
            batch.Draw(pixel, new Rectangle(0, 0, 1280, 720), Color.Black * 0.78f);

            if (menuTiles == null || menuTiles.Width < 256 || menuTiles.Height < 256)
            {
                batch.Draw(pixel, area, new Color(28, 58, 38));
                batch.Draw(pixel, new Rectangle(area.X, area.Y, area.Width, 4), new Color(126, 170, 92));
                batch.Draw(pixel, new Rectangle(area.X, area.Bottom - 4, area.Width, 4), new Color(126, 170, 92));
            }
            else
            {
                var t = menuTiles;
                batch.Draw(t, new Rectangle(area.X + 28, area.Y + 28, area.Width - 64, area.Height - 64),
                    new Rectangle(64, 128, 64, 64), Color.White);
                batch.Draw(t, new Vector2(area.X, area.Y), new Rectangle(0, 0, 64, 64), Color.White);
                batch.Draw(t, new Vector2(area.Right - 64, area.Y), new Rectangle(192, 0, 64, 64), Color.White);
                batch.Draw(t, new Vector2(area.Right - 64, area.Bottom - 64), new Rectangle(192, 192, 64, 64), Color.White);
                batch.Draw(t, new Vector2(area.X, area.Bottom - 64), new Rectangle(0, 192, 64, 64), Color.White);
                batch.Draw(t, new Rectangle(area.X + 64, area.Y, area.Width - 128, 64), new Rectangle(128, 0, 64, 64), Color.White);
                batch.Draw(t, new Rectangle(area.X + 64, area.Bottom - 64, area.Width - 128, 64), new Rectangle(128, 192, 64, 64), Color.White);
                batch.Draw(t, new Rectangle(area.X, area.Y + 64, 64, area.Height - 128), new Rectangle(0, 128, 64, 64), Color.White);
                batch.Draw(t, new Rectangle(area.Right - 64, area.Y + 64, 64, area.Height - 128), new Rectangle(192, 128, 64, 64), Color.White);
            }

            if (font != null && !string.IsNullOrEmpty(title))
                DrawShadowText(batch, title, dialogueFont ?? font,
                    new Vector2(area.X + 24, area.Y + 22), new Color(255, 236, 160), 1f);
        }

        /// <summary>Texto com a sombra laranja da 1.5.6.</summary>
        protected void DrawShadowText(SpriteBatch batch, string text, SpriteFont usedFont,
            Vector2 position, Color color, float scale)
        {
            if (usedFont == null || string.IsNullOrEmpty(text))
                return;
            batch.DrawString(usedFont, text, position + new Vector2(-2, 2), ShadowColor,
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            batch.DrawString(usedFont, text, position + new Vector2(0, 2), ShadowColor,
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            batch.DrawString(usedFont, text, position, color,
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
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
                DrawShadowText(batch, text, font, new Vector2(r.X + 18, r.Y + 10), TextColor, 1f);
        }

        protected void DrawBack(SpriteBatch batch)
        {
            DrawRow(batch, BackRect, Localization.CS("StringsFromCSFiles:TitleMenu.cs.11739", "Voltar"), false);
        }

        private static Texture2D LoadTexture(ContentManager content, params string[] assets)
        {
            foreach (var asset in assets)
            {
                try
                {
                    var result = content.Load<Texture2D>(asset);
                    if (result != null)
                        return result;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Asset indisponivel " + asset + ": " + ex.Message);
                }
            }
            return null;
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
