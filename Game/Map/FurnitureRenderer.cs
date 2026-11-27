using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Map
{
    /// <summary>
    /// Porte do Furniture.draw da 1.5.6. Moveis sao ITENS (Data/Furniture),
    /// nao tiles do mapa: por isso nao apareciam no renderizador de camadas.
    /// </summary>
    public sealed class FurnitureRenderer
    {
        private Texture2D texture;
        private Texture2D giftbox;
        private Dictionary<int, string> data;

        public void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>("TileSheets/furniture");
            data = content.Load<Dictionary<int, string>>("Data/Furniture");
            try { giftbox = content.Load<Texture2D>("LooseSprites/Giftbox"); }
            catch { giftbox = null; }
        }

        /// <summary>Chest giftbox: draw do original em LooseSprites/Giftbox.</summary>
        public void DrawGiftbox(SpriteBatch batch, int tileX, int tileY)
        {
            if (giftbox == null)
                return;
            batch.Draw(giftbox, new Vector2(tileX * 64f, tileY * 64f - 52f),
                new Rectangle(0, 0, 16, 32), Color.White, 0f, Vector2.Zero, 4f,
                SpriteEffects.None, (tileY * 64f) / 10000f);
        }

        public void Draw(SpriteBatch batch, int which, int tileX, int tileY)
        {
            if (texture == null || data == null)
                return;

            string row;
            if (!data.TryGetValue(which, out row))
                return;

            var parts = row.Split('/');
            int width, height;
            if (parts.Length > 2 && parts[2] != "-1")
            {
                var dims = parts[2].Split(' ');
                width = int.Parse(dims[0]);
                height = int.Parse(dims[1]);
            }
            else
            {
                var type = parts.Length > 1 ? TypeNumber(parts[1]) : 9;
                switch (type)
                {
                    case 1:
                    case 3:
                    case 4:
                    case 6: width = 2; height = 2; break;
                    case 2:
                    case 5:
                    case 10:
                    case 11: width = 3; height = 2; break;
                    case 7: width = 1; height = 3; break;
                    case 14: width = 2; height = 5; break;
                    default: width = 1; height = 2; break;
                }
            }

            var src = new Rectangle(
                which * 16 % texture.Width,
                which * 16 / texture.Width * 16,
                width * 16, height * 16);
            if (src.X < 0 || src.Y < 0 || src.Right > texture.Width || src.Bottom > texture.Height)
                return;

            // boundingBox: Data/Furniture[3] ou o padrao por tipo.
            int boxW = 1, boxH = 1;
            if (parts.Length > 3 && parts[3] != "-1")
            {
                var bb = parts[3].Split(' ');
                boxW = int.Parse(bb[0]);
                boxH = int.Parse(bb[1]);
            }
            else
            {
                var type = parts.Length > 1 ? TypeNumber(parts[1]) : 9;
                switch (type)
                {
                    case 1: case 3: case 4: case 10: case 14: boxW = 2; boxH = 1; break;
                    case 2: boxW = 3; boxH = 1; break;
                    case 5: boxW = 5; boxH = 2; break;
                    case 6: case 11: case 12: boxW = 3; boxH = 2; break;
                    case 17: case 13: boxW = 1; boxH = 2; break;
                    default: boxW = 1; boxH = 1; break;
                }
            }

            // updateDrawPosition: (boundingBox.X, Y - (src.Height*4 - box.Height)).
            var pos = new Vector2(tileX * 64f,
                tileY * 64f - (src.Height * 4f - boxH * 64f));
            batch.Draw(texture, pos, src, Color.White, 0f, Vector2.Zero, 4f,
                SpriteEffects.None, (tileY * 64f + boxH * 64f - 8f) / 10000f);
        }

        private static int TypeNumber(string name)
        {
            var lower = name.ToLowerInvariant();
            if (lower.StartsWith("bed")) return 15;
            switch (lower)
            {
                case "chair": return 0;
                case "bench": return 1;
                case "couch": return 2;
                case "armchair": return 3;
                case "dresser": return 4;
                case "long table": return 5;
                case "painting": return 6;
                case "lamp": return 7;
                case "decor": return 8;
                case "bookcase": return 10;
                case "table": return 11;
                case "rug": return 12;
                case "window": return 13;
                case "fireplace": return 14;
                case "torch": return 16;
                case "sconce": return 17;
                default: return 9;
            }
        }
    }
}
