using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Map
{
    /// <summary>
    /// Porte 1:1 de StardewValley.Objects.Furniture.draw (1.5.6, linhas
    /// 1461-1517) e getDefaultSourceRectForType/getDefaultBoundingBoxForType.
    /// Moveis sao ITENS (Data/Furniture), nao tiles do mapa.
    /// </summary>
    public sealed class FurnitureRenderer
    {
        private Texture2D texture;
        private Texture2D giftbox;
        private Texture2D shadow;
        private Texture2D cursors;
        private Texture2D objects;
        private Dictionary<int, string> data;

        // Parte 1/2/3/4 de Data/Furniture, exatamente como no original.
        private sealed class Def
        {
            public string Name;
            public int Type;
            public int Width;
            public int Height;
            public int BoxW;
            public int BoxH;
        }

        public void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>("TileSheets/furniture");
            data = content.Load<Dictionary<int, string>>("Data/Furniture");
            TryLoad(content, "LooseSprites/Giftbox", out giftbox);
            TryLoad(content, "LooseSprites/shadow", out shadow);
            TryLoad(content, "LooseSprites/Cursors.pt-BR", out cursors);
            if (cursors == null) TryLoad(content, "LooseSprites/Cursors", out cursors);
            TryLoad(content, "TileSheets/Objects", out objects);
        }

        private static void TryLoad(ContentManager content, string name, out Texture2D result)
        {
            try { result = content.Load<Texture2D>(name); }
            catch { result = null; }
        }

        private Def GetDef(int which)
        {
            string row;
            if (data == null || !data.TryGetValue(which, out row))
                return null;
            var parts = row.Split('/');
            var def = new Def
            {
                Name = parts.Length > 0 ? parts[0] : "",
                Type = parts.Length > 1 ? TypeNumber(parts[1]) : 9,
                BoxW = 1,
                BoxH = 1
            };
            if (parts.Length > 2 && parts[2] != "-1")
            {
                var dims = parts[2].Split(' ');
                def.Width = int.Parse(dims[0]);
                def.Height = int.Parse(dims[1]);
            }
            else
            {
                def.Width = DefaultWidth(def.Type);
                def.Height = DefaultHeight(def.Type);
            }
            if (parts.Length > 3 && parts[3] != "-1")
            {
                var bb = parts[3].Split(' ');
                def.BoxW = int.Parse(bb[0]);
                def.BoxH = int.Parse(bb[1]);
            }
            else
            {
                def.BoxW = DefaultBoxWidth(def.Type);
                def.BoxH = DefaultBoxHeight(def.Type);
            }
            return def;
        }

        // getDefaultSourceRectForType (linhas 1191-1244).
        private static int DefaultWidth(int t)
        {
            switch (t)
            {
                case 1: case 3: case 4: case 6: return 2;
                case 2: case 12: return 3;
                case 5: return 5;
                case 10: case 11: return 2;
                case 7: return 1;
                case 14: return 2;
                default: return 1;
            }
        }

        private static int DefaultHeight(int t)
        {
            switch (t)
            {
                case 1: case 3: case 4: case 6: return 2;
                case 2: case 12: return 2;
                case 5: return 3;
                case 10: case 11: return 3;
                case 7: return 3;
                case 14: return 5;
                default: return 2;
            }
        }

        private static int DefaultBoxWidth(int t)
        {
            switch (t)
            {
                case 1: case 3: case 4: case 10: case 14: return 2;
                case 2: case 12: return 3;
                case 5: return 5;
                case 6: case 11: return 2;
                case 17: case 13: return 1;
                default: return 1;
            }
        }

        private static int DefaultBoxHeight(int t)
        {
            switch (t)
            {
                case 1: case 3: case 4: case 10: case 14: return 1;
                case 2: return 1;
                case 5: return 2;
                case 6: case 11: case 12: return 2;
                case 17: case 13: return 2;
                default: return 1;
            }
        }

        private Rectangle SourceRect(int which, Def def, int sourceIndexOffset)
        {
            // value.X += value.Width * sourceIndexOffset (Furniture.draw 1467-1468).
            return new Rectangle(
                which * 16 % texture.Width + def.Width * 16 * sourceIndexOffset,
                which * 16 / texture.Width * 16,
                def.Width * 16, def.Height * 16);
        }

        /// <summary>Caixa de colisao em pixels do mundo (Furniture.getBoundingBox).</summary>
        public Rectangle GetBoundingBox(int which, int tileX, int tileY)
        {
            var def = GetDef(which);
            if (def == null)
                return Rectangle.Empty;
            return new Rectangle(tileX * 64, tileY * 64, def.BoxW * 64, def.BoxH * 64);
        }

        /// <summary>Tapetes (tipo 12) sao passaveis, como no original.</summary>
        public bool IsPassable(int which)
        {
            var def = GetDef(which);
            return def != null && def.Type == 12;
        }

        /// <summary>
        /// Porte de BedFurniture.IntersectsForCollision (linhas 400-417): a cama
        /// so bloqueia na cabeca (64px) e nos pes (Y+128 em diante). O meio fica
        /// livre para o jogador entrar e ativar a pergunta de dormir.
        /// </summary>
        public bool IntersectsForCollision(int which, int tileX, int tileY, Rectangle box)
        {
            var def = GetDef(which);
            if (def == null)
                return false;
            var full = GetBoundingBox(which, tileX, tileY);
            if (def.Type != 15)
                return full.Intersects(box);

            var head = full;
            head.Height = 64;
            if (head.Intersects(box))
                return true;
            var foot = full;
            foot.Y += 128;
            foot.Height -= 128;
            return foot.Intersects(box);
        }

        /// <summary>Porte literal de Furniture.draw(spriteBatch, x, y, alpha).</summary>
        public void Draw(SpriteBatch batch, int which, int tileX, int tileY,
            int sourceIndexOffset = 0, bool flipped = false, bool isOn = false,
            bool drawHeldObjectLow = false, bool shake = false, double timeMs = 0,
            int heldObject = -1)
        {
            if (texture == null)
                return;
            var def = GetDef(which);
            if (def == null)
                return;

            var src = SourceRect(which, def, sourceIndexOffset);
            if (src.X < 0 || src.Y < 0 || src.Right > texture.Width || src.Bottom > texture.Height)
                return;

            var box = GetBoundingBox(which, tileX, tileY);
            var jitter = shake ? new Vector2((int)timeMs % 3 - 1, (int)(timeMs / 3) % 3 - 1) : Vector2.Zero;
            var pos = new Vector2(tileX * 64f + jitter.X,
                tileY * 64f - (src.Height * 4f - box.Height) + jitter.Y);

            // Layer depth exata: tapete usa 2e-9 + Y/100000; tipos 6/17/13
            // usam Bottom-48; os demais Bottom-8 (linha 1478/1483).
            float depth;
            if (def.Type == 12)
                depth = 2E-09f + tileY / 100000f;
            else
            {
                var bottomShift = (def.Type == 6 || def.Type == 17 || def.Type == 13) ? 48 : 8;
                depth = (box.Bottom - bottomShift) / 10000f;
            }

            var effects = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            batch.Draw(texture, pos, src, Color.White, 0f, Vector2.Zero, 4f, effects, depth);

            // heldObject: sombra + sprite do objeto (linhas 1485-1495).
            if (heldObject >= 0)
            {
                var heldDef = GetDef(heldObject);
                var center = new Vector2(box.Center.X, box.Center.Y);
                if (heldDef != null)
                {
                    var heldSrc = SourceRect(heldObject, heldDef, 0);
                    var lowY = drawHeldObjectLow ? -16 : 16;
                    var heldPos = new Vector2(center.X - 32f,
                        center.Y - heldSrc.Height * 4f - lowY);
                    batch.Draw(texture, heldPos, heldSrc, Color.White, 0f, Vector2.Zero, 4f,
                        SpriteEffects.None, (box.Bottom - 7) / 10000f);
                }
                else if (objects != null)
                {
                    var lowY = drawHeldObjectLow ? 32 : 85;
                    if (shadow != null)
                        batch.Draw(shadow, new Vector2(center.X - 32f, center.Y - lowY) + new Vector2(32f, 53f),
                            shadow.Bounds, Color.White, 0f,
                            new Vector2(shadow.Bounds.Center.X, shadow.Bounds.Center.Y),
                            4f, SpriteEffects.None, box.Bottom / 10000f);
                    // getSourceRectForObject: 16x16 por indice, 16 colunas.
                    var oSrc = new Rectangle(heldObject % 16 * 16, heldObject / 16 * 16, 16, 16);
                    batch.Draw(objects, new Vector2(center.X - 32f, center.Y - lowY), oSrc,
                        Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None,
                        (box.Bottom + 1) / 10000f);
                }
            }

            // Chamas animadas de lareira (14) e tocha (16), linhas 1497-1505.
            if (isOn && cursors != null && (def.Type == 14 || def.Type == 16))
            {
                int phase = (int)((timeMs + tileX * 3047 + tileY * 88) % 400.0 / 100.0);
                if (def.Type == 14)
                {
                    var f1 = new Rectangle(276 + phase * 12, 1985, 12, 11);
                    var f2 = new Rectangle(276 + (int)((timeMs + tileX * 2047 + tileY * 98) % 400.0 / 100.0) * 12, 1985, 12, 11);
                    batch.Draw(cursors, new Vector2(box.Center.X - 12f, box.Center.Y - 64f), f1,
                        Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (box.Bottom - 2) / 10000f);
                    batch.Draw(cursors, new Vector2(box.Center.X - 36f, box.Center.Y - 64f), f2,
                        Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (box.Bottom - 1) / 10000f);
                }
                else
                {
                    batch.Draw(cursors, new Vector2(box.Center.X - 20f, box.Center.Y - 105.6f),
                        new Rectangle(276 + phase * 12, 1985, 12, 11), Color.White, 0f,
                        Vector2.Zero, 4f, SpriteEffects.None, (box.Bottom - 2) / 10000f);
                }
            }
        }

        /// <summary>Chest giftbox: draw do original em LooseSprites/Giftbox.</summary>
        public void DrawGiftbox(SpriteBatch batch, int tileX, int tileY)
        {
            if (giftbox == null)
                return;
            if (shadow != null)
                batch.Draw(shadow, new Vector2(tileX * 64f + 16f, tileY * 64f + 53f),
                    shadow.Bounds, Color.White, 0f,
                    new Vector2(shadow.Bounds.Center.X, shadow.Bounds.Center.Y),
                    5f, SpriteEffects.None, 0.0000001f);
            batch.Draw(giftbox, new Vector2(tileX * 64f, tileY * 64f - 52f),
                new Rectangle(0, 0, 16, 32), Color.White, 0f, Vector2.Zero, 4f,
                SpriteEffects.None, tileY * 64f / 10000f);
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
