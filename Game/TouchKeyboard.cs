using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game
{
    /// <summary>
    /// Teclado touch desenhado pelo proprio jogo. Evita depender do teclado
    /// virtual do Windows dentro de uma superficie SwapChainPanel e mantem a
    /// interacao de um unico toque por tecla.
    /// </summary>
    internal sealed class TouchKeyboard
    {
        private sealed class Key
        {
            public Rectangle Bounds;
            public string Text;
        }

        private const int MaxLength = 24;

        private static readonly string[] Rows =
        {
            "1234567890",
            "QWERTYUIOP",
            "ASDFGHJKL",
            "ZXCVBNM"
        };

        private readonly List<Key> keys = new List<Key>();
        private Rectangle shiftKey;
        private Rectangle backKey;
        private Rectangle spaceKey;
        private Rectangle okKey;

        public bool Visible { get; private set; }
        public bool Shift { get; private set; }

        public Rectangle Bounds
        {
            get { return new Rectangle(340, 396, 600, 296); }
        }

        public TouchKeyboard()
        {
            Build();
        }

        private void Build()
        {
            for (var row = 0; row < Rows.Length; row++)
            {
                var text = Rows[row];
                var width = 52;
                var gap = 6;
                var total = text.Length * width + (text.Length - 1) * gap;
                var startX = Bounds.X + (Bounds.Width - total) / 2;
                var y = Bounds.Y + 44 + row * 48;

                for (var i = 0; i < text.Length; i++)
                {
                    keys.Add(new Key
                    {
                        Bounds = new Rectangle(startX + i * (width + gap), y, width, 44),
                        Text = text.Substring(i, 1)
                    });
                }
            }

            shiftKey = new Rectangle(Bounds.X + 12, Bounds.Y + 248, 84, 40);
            backKey = new Rectangle(Bounds.X + 104, Bounds.Y + 248, 104, 40);
            spaceKey = new Rectangle(Bounds.X + 216, Bounds.Y + 248, 216, 40);
            okKey = new Rectangle(Bounds.X + 440, Bounds.Y + 248, 148, 40);

            keys.Add(new Key { Bounds = shiftKey, Text = "Shift" });
            keys.Add(new Key { Bounds = backKey, Text = "Back" });
            keys.Add(new Key { Bounds = spaceKey, Text = "Space" });
            keys.Add(new Key { Bounds = okKey, Text = "OK" });
        }

        public void Show()
        {
            Visible = true;
            Shift = false;
        }

        public void Hide()
        {
            Visible = false;
            Shift = false;
        }

        /// <summary>Processa um toque e devolve o texto resultante do campo.</summary>
        public string HandleTap(Vector2 point, string current)
        {
            if (!Visible || current == null)
                return current ?? string.Empty;

            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                if (!key.Bounds.Contains((int)point.X, (int)point.Y))
                    continue;

                if (key.Text == "Shift")
                {
                    Shift = !Shift;
                    return current;
                }

                if (key.Text == "Back")
                {
                    if (current.Length > 0)
                        current = current.Substring(0, current.Length - 1);
                    return current;
                }

                if (key.Text == "Space")
                {
                    if (current.Length < MaxLength)
                        current += " ";
                    return current;
                }

                if (key.Text == "OK")
                {
                    Hide();
                    return current;
                }

                if (current.Length >= MaxLength)
                    return current;

                var letter = key.Text[0];
                if (Shift && char.IsLetter(letter))
                    letter = char.ToUpperInvariant(letter);
                else
                    letter = char.ToLowerInvariant(letter);

                current += letter;
                Shift = false;
                return current;
            }

            return current;
        }

        public void Draw(SpriteBatch batch, Texture2D pixel, SpriteFont font, string hint)
        {
            if (pixel == null || !Visible)
                return;

            batch.Draw(pixel, Bounds, new Color(12, 28, 20) * 0.97f);
            batch.Draw(pixel, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 3), new Color(126, 170, 92));

            if (font != null && !string.IsNullOrEmpty(hint))
            {
                batch.DrawString(font, hint, new Vector2(Bounds.X + 14, Bounds.Y + 14),
                    new Color(255, 236, 160));
            }

            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                var active = key.Text == "Shift" && Shift;
                var color = active ? new Color(118, 172, 88) : new Color(48, 82, 54);
                batch.Draw(pixel, key.Bounds, color);
                batch.Draw(pixel, new Rectangle(key.Bounds.X, key.Bounds.Y, key.Bounds.Width, 2),
                    new Color(126, 170, 92));

                if (font == null)
                    continue;

                var label = key.Text == "Back" ? "<" : key.Text;
                var size = font.MeasureString(label);
                var x = key.Bounds.X + (key.Bounds.Width - size.X) / 2f;
                var y = key.Bounds.Y + (key.Bounds.Height - size.Y) / 2f;
                batch.DrawString(font, label, new Vector2(x, y), Color.White);
            }
        }
    }
}
