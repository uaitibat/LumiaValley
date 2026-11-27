using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game
{
    /// <summary>
    /// Porte 1:1 de StardewValley.Menus.DayTimeMoneyBox.draw (linhas 205-434).
    /// Idioma pt: horas em 24h com zero a esquerda e sem AM/PM (linhas 287-302).
    /// </summary>
    public sealed class DayTimeMoneyBoxPort
    {
        private Texture2D cursors;
        private SpriteFont font;
        private SpriteFont small;
        private readonly Rectangle sourceRect = new Rectangle(333, 431, 71, 43);
        private string lastDayString;
        private int lastDay = -1;

        public void Load(ContentManager content)
        {
            try { cursors = content.Load<Texture2D>("LooseSprites/Cursors.pt-BR"); } catch { }
            if (cursors == null) try { cursors = content.Load<Texture2D>("LooseSprites/Cursors"); } catch { }
            try { font = content.Load<SpriteFont>("Fonts/SmallFont.pt-BR"); } catch { }
            if (font == null) try { font = content.Load<SpriteFont>("Fonts/SmallFont"); } catch { }
            try { small = content.Load<SpriteFont>("Fonts/tinyFont"); } catch { }
        }

        public void Draw(SpriteBatch b, int viewportWidth, int timeOfDay, int dayOfMonth,
            string season, int weatherIcon, int money, int gameTimeInterval,
            int offsetX = 0, int offsetY = 0, int worldWidth = 0)
        {
            if (cursors == null)
                return;
            // DayTimeMoneyBox.updatePosition: position = viewport.Width - 300.
            // No Lumia o HUD fica no canto direito DO VIEWPORT (fora da casa),
            // para nao sobrepor a cena centralizada.
            var position = new Vector2(viewportWidth - 300 + offsetX, 8f + offsetY);
            b.Draw(cursors, position, sourceRect, Color.White, 0f, Vector2.Zero, 4f,
                SpriteEffects.None, 0.9f);

            if (font != null)
            {
                if (dayOfMonth != lastDay)
                {
                    lastDay = dayOfMonth;
                    lastDayString = ShortDay(dayOfMonth);
                }
                var dateText = lastDayString + ". " + dayOfMonth;
                var ds = font.MeasureString(dateText);
                var dOff = new Vector2(sourceRect.X * 0.55f - ds.X / 2f,
                                       sourceRect.Y * 0.1f - ds.Y / 2f);
                DrawShadowText(b, dateText, font, position + dOff, new Color(96, 80, 64));

                // Horas em pt (linhas 287-302): HH de 24h com 0 a esquerda.
                var hours = timeOfDay / 100 % 24;
                var timeText = (hours <= 9 ? "0" : "") + hours + ":" +
                               (timeOfDay % 100 == 0 ? "0" : "") + (timeOfDay % 100);
                var ts = font.MeasureString(timeText);
                var tOff = new Vector2(sourceRect.X * 0.55f - ts.X / 2f,
                                       sourceRect.Y * 0.31f - ts.Y / 2f);
                DrawShadowText(b, timeText, font, position + tOff,
                    timeOfDay >= 2400 ? Color.Red : new Color(96, 80, 64));
            }

            var seasonNumber = SeasonNumber(season);
            b.Draw(cursors, position + new Vector2(212f, 68f),
                new Rectangle(406, 441 + seasonNumber * 8, 12, 8), Color.White,
                0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
            b.Draw(cursors, position + new Vector2(116f, 68f),
                new Rectangle(317 + 12 * weatherIcon, 421, 12, 8), Color.White,
                0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);

            // drawMoneyBox (linhas 157-166).
            b.Draw(cursors, position + new Vector2(28f, 172f),
                new Rectangle(340, 472, 65, 17), Color.White, 0f, Vector2.Zero, 4f,
                SpriteEffects.None, 0.9f);
            if (font != null)
            {
                var moneyText = money + "g";
                var ms = font.MeasureString(moneyText);
                // Caixa em position+(28,172), tamanho 65*4=260 por 17*4=68.
                // Centraliza o valor dentro dela.
                DrawShadowText(b, moneyText, font,
                    position + new Vector2(28f + (260f - ms.X) / 2f, 172f + (68f - ms.Y) / 2f),
                    new Color(255, 228, 120));
            }

            // Ponteiro do relogio (linha 433).
            var clock = (timeOfDay - timeOfDay % 100) + (timeOfDay % 100 / 10) * 16.66f;
            var angle = Math.PI + Math.Min(Math.PI,
                ((clock + gameTimeInterval / 7000f * 16.6f - 600f) / 2000f) * Math.PI);
            b.Draw(cursors, position + new Vector2(88f, 88f),
                new Rectangle(324, 477, 7, 19), Color.White, (float)angle,
                new Vector2(3f, 17f), 4f, SpriteEffects.None, 0.9f);
        }

        private static int SeasonNumber(string season)
        {
            switch (season)
            {
                case "summer": return 1;
                case "fall": return 2;
                case "winter": return 3;
                default: return 0;
            }
        }

        private static string ShortDay(int day)
        {
            var names = new[] { "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb", "Dom" };
            return names[(day - 1) % 7];
        }

        private static void DrawShadowText(SpriteBatch b, string text, SpriteFont f,
            Vector2 pos, Color color)
        {
            b.DrawString(f, text, pos + new Vector2(2f, 2f), Color.Black * 0.6f);
            b.DrawString(f, text, pos, color);
        }
    }

    /// <summary>
    /// Porte 1:1 de StardewValley.Menus.Toolbar.draw (linhas 123-190).
    /// 12 slots, fundo MenuTiles(0,256,60,60), icones de tool/weapon.
    /// </summary>
    public sealed class ToolbarPort
    {
        private Texture2D menu;
        private Texture2D tools;
        private Texture2D weapons;
        private SpriteFont tiny;
        private static readonly string[] SlotText =
            { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" };

        // Farmer.initialTools() (linhas 2300-2310): Axe, Hoe, WateringCan,
        // Pickaxe, MeleeWeapon(47). O menu usa IndexOfMenuItemView, NAO os
        // indices 0..3 (Tool.cs:1126-1152):
        //   Axe   = 189 + 5 + 21 = 215
        //   Hoe   =  21 + 5 + 21 = 47
        //   Can   = 273 + 2 + 21 = 296
        //   Pick  = 105 + 5 + 21 = 131
        // MeleeWeapon(47): weaponsTexture com IndexOfMenuItemView = 47.
        private static readonly int[] ToolIcon = { 215, 47, 296, 131, -1 };

        public void Load(ContentManager content)
        {
            try { menu = content.Load<Texture2D>("Maps/MenuTiles"); } catch { }
            try { tools = content.Load<Texture2D>("TileSheets/tools"); } catch { }
            try { weapons = content.Load<Texture2D>("TileSheets/weapons"); } catch { }
            try { tiny = content.Load<SpriteFont>("Fonts/tinyFont"); } catch { }
        }

        private static Rectangle StdRect(Texture2D t, int index, int size = 16)
        {
            return new Rectangle(index * size % t.Width, index * size / t.Width * size,
                size, size);
        }

        public void Draw(SpriteBatch b, int viewportWidth, int viewportHeight, int selected,
            int offsetX = 0, int offsetY = 0, int worldCenter = 0)
        {
            if (menu == null)
                return;
            var y = viewportHeight + 8 + offsetY;
            var center = worldCenter > 0 ? worldCenter : viewportWidth / 2;
            var x0 = center - 384 - 16 + offsetX;

            // IClickableMenu.drawTextureBox (simplificado sem sombra, como
            // Toolbar chama drawShadow: false).
            var src = new Rectangle(0, 256, 60, 60);
            var n = src.Width / 3; // 20
            b.Draw(menu, new Rectangle(x0 + n, y - 96 - 8 + n, 800 - n * 2, 96 - n * 2),
                new Rectangle(src.X + n, src.Y + n, n, n), Color.White);
            b.Draw(menu, new Vector2(x0, y - 96 - 8), new Rectangle(src.X, src.Y, n, n),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            b.Draw(menu, new Vector2(x0 + 800 - n, y - 96 - 8),
                new Rectangle(src.X + n * 2, src.Y, n, n), Color.White);
            b.Draw(menu, new Rectangle(x0 + n, y - 96 - 8, 800 - n * 2, n),
                new Rectangle(src.X + n, src.Y, n, n), Color.White);
            b.Draw(menu, new Rectangle(x0, y - 96 - 8 + n, n, 96 - n * 2),
                new Rectangle(src.X, src.Y + n, n, n), Color.White);

            for (var j = 0; j < 12; j++)
            {
                var sx = center - 384 + j * 64 + offsetX;
                var sy = y - 96 + 8;
                b.Draw(menu, new Vector2(sx, sy),
                    StdRect(menu, selected == j ? 56 : 10, 16), Color.White);
                if (tiny != null)
                    b.DrawString(tiny, SlotText[j], new Vector2(sx + 4, sy - 8), Color.DimGray);
            }

            for (var k = 0; k < ToolIcon.Length; k++)
            {
                var sx = center - 384 + k * 64 + offsetX;
                var sy = y - 96 + 8;
                var scale = selected == k ? 0.9f : 0.8f;
                if (ToolIcon[k] < 0)
                {
                    if (weapons != null)
                        b.Draw(weapons, new Vector2(sx + 32, sy + 32),
                            StdRect(weapons, 47, 16), Color.White, 0f,
                            new Vector2(8f, 8f), scale * 4f, SpriteEffects.None, 0f);
                }
                else if (tools != null)
                {
                    b.Draw(tools, new Vector2(sx + 32, sy + 32),
                        StdRect(tools, ToolIcon[k], 16), Color.White, 0f,
                        new Vector2(8f, 8f), scale * 4f, SpriteEffects.None, 0f);
                }
            }
        }

        /// <summary>
        /// Layout vertical para o Lumia (mobile), excecao deliberada ao layout
        /// horizontal do desktop. Usa os MESMOS slots, sourceRects e ícones
        /// 1:1 de Toolbar.draw; muda apenas a posicao dos 12 botoes.
        /// 12 slots de 48px cabem em 720px de altura com margem.
        /// </summary>
        public void DrawVertical(SpriteBatch b, int viewportHeight, int selected,
            int offsetX = 0, int offsetY = 0)
        {
            if (menu == null)
                return;
            const int slot = 48;
            const int x = 8;
            const float scale = 0.75f; // 64 -> 48
            var total = slot * 12;
            var startY = (viewportHeight - total) / 2;

            // Fundo simples translucido (sem o drawTextureBox, que com 12
            // slots verticais virava uma barra grossa e bugada).
            b.Draw(menu, new Rectangle(x, startY, 56, total),
                new Rectangle(0, 256, 60, 60), Color.White * 0.55f);

            for (var j = 0; j < 12; j++)
            {
                var sx = x + 4 + offsetX;
                var sy = startY + j * slot + offsetY;
                b.Draw(menu, new Vector2(sx, sy), StdRect(menu, selected == j ? 56 : 10, 16),
                    Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            for (var k = 0; k < ToolIcon.Length; k++)
            {
                var sx = x + 4 + offsetX;
                var sy = startY + k * slot + offsetY;
                var s = (selected == k ? 0.9f : 0.8f) * scale * 4f;
                if (ToolIcon[k] < 0)
                {
                    if (weapons != null)
                        b.Draw(weapons, new Vector2(sx + 24, sy + 24),
                            StdRect(weapons, 47, 16), Color.White, 0f,
                            new Vector2(8f, 8f), s, SpriteEffects.None, 0f);
                }
                else if (tools != null)
                {
                    b.Draw(tools, new Vector2(sx + 24, sy + 24),
                        StdRect(tools, ToolIcon[k], 16), Color.White, 0f,
                        new Vector2(8f, 8f), s, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
