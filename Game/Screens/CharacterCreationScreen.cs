using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Criacao de personagem e escolha de fazenda, reimplementada a partir do
    /// comportamento e das regioes de sprite da 1.5.6. Nao ha codigo copiado
    /// do jogo: apenas os assets oficiais carregados pelo ContentManager.
    /// </summary>
    public sealed class CharacterCreationScreen : MenuScreen
    {
        private static readonly Rectangle Panel = new Rectangle(284, 8, 712, 704);
        private static readonly Rectangle Portrait = new Rectangle(316, 80, 144, 216);
        private static readonly Rectangle NameRect = new Rectangle(500, 68, 464, 44);
        private static readonly Rectangle FarmNameRect = new Rectangle(500, 122, 464, 44);
        private static readonly Rectangle FavoriteRect = new Rectangle(500, 176, 464, 44);
        private static readonly Rectangle MaleRect = new Rectangle(304, 312, 64, 52);
        private static readonly Rectangle FemaleRect = new Rectangle(384, 312, 64, 52);
        private static readonly Rectangle PetNoneRect = new Rectangle(304, 382, 64, 44);
        private static readonly Rectangle PetDogRect = new Rectangle(384, 382, 64, 44);
        private static readonly Rectangle PetCatRect = new Rectangle(464, 382, 64, 44);
        private static readonly Rectangle FarmInfoRect = new Rectangle(300, 468, 230, 152);
        private static readonly Rectangle OkRect = new Rectangle(884, 628, 92, 56);

        private static readonly Color[] HairPalette =
        {
            new Color(110, 58, 30), new Color(129, 66, 36), new Color(155, 79, 43),
            new Color(193, 90, 50), new Color(224, 137, 83), new Color(246, 191, 121),
            new Color(255, 222, 170), new Color(148, 118, 92), new Color(94, 73, 54),
            new Color(75, 49, 32), new Color(45, 35, 25), new Color(241, 174, 73)
        };

        private static readonly Color[] ShirtPalette =
        {
            new Color(211, 215, 211), new Color(88, 110, 170), new Color(64, 86, 150),
            new Color(50, 62, 120), new Color(148, 76, 70), new Color(118, 60, 56),
            new Color(80, 44, 40), new Color(192, 148, 92), new Color(148, 106, 64),
            new Color(96, 72, 48), new Color(212, 176, 92), new Color(164, 128, 64),
            new Color(112, 148, 96), new Color(72, 112, 80), new Color(48, 80, 60),
            new Color(126, 92, 140), new Color(92, 64, 108), new Color(64, 44, 76),
            new Color(180, 180, 180), new Color(64, 64, 64)
        };

        private static readonly Color[] PantsPalette =
        {
            new Color(64, 86, 150), new Color(46, 85, 183), new Color(38, 64, 128),
            new Color(88, 110, 170), new Color(148, 76, 70), new Color(118, 60, 56),
            new Color(80, 44, 40), new Color(96, 72, 48), new Color(112, 148, 96),
            new Color(72, 112, 80), new Color(48, 80, 60), new Color(64, 64, 64),
            new Color(126, 92, 140), new Color(180, 180, 180), new Color(192, 148, 92),
            new Color(164, 128, 64), new Color(92, 64, 108), new Color(64, 44, 76),
            new Color(50, 62, 120), new Color(211, 215, 211)
        };

        private static readonly Color[] SkinFallback =
        {
            new Color(232, 186, 156), new Color(224, 172, 140), new Color(211, 158, 128),
            new Color(198, 146, 116), new Color(184, 134, 104), new Color(170, 122, 92),
            new Color(156, 110, 82), new Color(142, 98, 72), new Color(128, 86, 62),
            new Color(114, 74, 54), new Color(100, 64, 46), new Color(86, 54, 38),
            new Color(238, 196, 168), new Color(226, 182, 152), new Color(214, 166, 138),
            new Color(202, 152, 124), new Color(188, 138, 110), new Color(174, 124, 96),
            new Color(160, 110, 84), new Color(146, 96, 72), new Color(132, 84, 62),
            new Color(118, 72, 54), new Color(104, 62, 46), new Color(90, 52, 38)
        };

        private static readonly Color[] ShoeFallback =
        {
            new Color(64, 64, 64), new Color(96, 64, 40), new Color(128, 80, 48),
            new Color(160, 112, 64), new Color(192, 160, 112), new Color(224, 208, 176),
            new Color(211, 215, 211), new Color(150, 150, 150), new Color(110, 110, 110),
            new Color(80, 80, 80), new Color(52, 52, 52), new Color(32, 32, 32),
            new Color(112, 72, 120), new Color(72, 96, 140), new Color(72, 128, 96),
            new Color(140, 96, 64), new Color(180, 140, 96), new Color(210, 180, 140),
            new Color(240, 220, 190)
        };

        private readonly SaveSlot draft = new SaveSlot();
        private readonly TouchKeyboard keyboard = new TouchKeyboard();
        private int activeField;

        private Texture2D cursors;
        private Texture2D dayBackground;
        private Texture2D maleBase;
        private Texture2D femaleBase;
        private Texture2D hairTexture;
        private Texture2D shirtTexture;
        private Texture2D pantsTexture;
        private Texture2D accessoryTexture;
        private Color[] skinColors;
        private Color[] shoeColors;

        public override void Load(ContentManager content, GraphicsDevice device)
        {
            base.Load(content, device);

            cursors = LoadTexture(content, "LooseSprites/Cursors");
            dayBackground = LoadTexture(content, "LooseSprites/daybg");
            maleBase = LoadTexture(content, "Characters/Farmer/farmer_base");
            femaleBase = LoadTexture(content, "Characters/Farmer/farmer_girl_base");
            hairTexture = LoadTexture(content, "Characters/Farmer/hairstyles");
            shirtTexture = LoadTexture(content, "Characters/Farmer/shirts");
            pantsTexture = LoadTexture(content, "Characters/Farmer/pants");
            accessoryTexture = LoadTexture(content, "Characters/Farmer/accessories");

            var skinTexture = LoadTexture(content, "Characters/Farmer/skinColors");
            var shoeTexture = LoadTexture(content, "Characters/Farmer/shoeColors");
            skinColors = ReadPalette(skinTexture, 3, 24, SkinFallback);
            shoeColors = ReadPalette(shoeTexture, 4, 19, ShoeFallback);
        }

        private static Color[] ReadPalette(Texture2D texture, int group, int count, Color[] fallback)
        {
            var result = new Color[count];
            if (texture == null)
            {
                Array.Copy(fallback, result, count);
                return result;
            }

            try
            {
                var data = new Color[texture.Width * texture.Height];
                texture.GetData(data);
                for (var i = 0; i < count; i++)
                {
                    var index = i * group;
                    result[i] = index < data.Length ? data[index] : fallback[i % fallback.Length];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao ler paleta: " + ex.Message);
                Array.Copy(fallback, result, count);
            }

            return result;
        }

        public override IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (!tap.HasValue)
                return this;

            var point = tap.Value;

            if (keyboard.Visible)
            {
                if (keyboard.Bounds.Contains((int)point.X, (int)point.Y))
                {
                    switch (activeField)
                    {
                        case 1: draft.FarmerName = keyboard.HandleTap(point, draft.FarmerName); break;
                        case 2: draft.FarmName = keyboard.HandleTap(point, draft.FarmName); break;
                        case 3: draft.FavoriteThing = keyboard.HandleTap(point, draft.FavoriteThing); break;
                    }

                    if (!keyboard.Visible)
                        activeField = 0;
                    return this;
                }

                keyboard.Hide();
                activeField = 0;
                return this;
            }

            if (BackTapped(tap))
                return Home();

            if (Contains(NameRect, point))
            {
                activeField = 1;
                keyboard.Show();
                return this;
            }
            if (Contains(FarmNameRect, point))
            {
                activeField = 2;
                keyboard.Show();
                return this;
            }
            if (Contains(FavoriteRect, point))
            {
                activeField = 3;
                keyboard.Show();
                return this;
            }

            if (Contains(MaleRect, point)) { draft.IsMale = true; return this; }
            if (Contains(FemaleRect, point)) { draft.IsMale = false; return this; }
            if (Contains(PetNoneRect, point)) { draft.Pet = 0; return this; }
            if (Contains(PetDogRect, point)) { draft.Pet = 1; return this; }
            if (Contains(PetCatRect, point)) { draft.Pet = 2; return this; }

            for (var i = 0; i < FarmTypes.Count; i++)
            {
                if (Contains(FarmButtonRect(i), point))
                {
                    draft.FarmType = i;
                    return this;
                }
            }

            for (var i = 0; i < 12; i++)
            {
                var row = OptionRect(i);
                if (!Contains(row, point))
                    continue;

                var delta = Contains(new Rectangle(row.Right - 40, row.Y, 40, row.Height), point) ? 1 : -1;
                Adjust(i, delta);
                return this;
            }

            if (Contains(OkRect, point))
            {
                if (string.IsNullOrWhiteSpace(draft.FarmerName))
                    draft.FarmerName = "Fazendeiro";
                if (string.IsNullOrWhiteSpace(draft.FarmName))
                    draft.FarmName = "Fazenda";

                SaveStore.Create(draft);
                return new FarmScreen(draft);
            }

            return this;
        }

        private static Rectangle FarmButtonRect(int index)
        {
            return new Rectangle(1008, 48 + index * 88, 88, 80);
        }

        private static Rectangle OptionRect(int index)
        {
            var column = index % 2;
            var row = index / 2;
            return new Rectangle(500 + column * 240, 236 + row * 38, 232, 34);
        }

        private void Adjust(int index, int delta)
        {
            switch (index)
            {
                case 0: draft.Skin = Wrap(draft.Skin + delta, skinColors.Length); break;
                case 1: draft.Hair = Wrap(draft.Hair + delta, 96); break;
                case 2: draft.HairColor = Wrap(draft.HairColor + delta, HairPalette.Length); break;
                case 3: draft.Shirt = Wrap(draft.Shirt + delta, 200); break;
                case 4: draft.ShirtColor = Wrap(draft.ShirtColor + delta, ShirtPalette.Length); break;
                case 5: draft.Pants = Wrap(draft.Pants + delta, 20); break;
                case 6: draft.PantsColor = Wrap(draft.PantsColor + delta, PantsPalette.Length); break;
                case 7: draft.Shoes = Wrap(draft.Shoes + delta, shoeColors.Length); break;
                case 8:
                    if (draft.Accessory < 0 && delta > 0) draft.Accessory = 0;
                    else draft.Accessory = Wrap(draft.Accessory + delta, 20);
                    break;
                case 9: draft.Pet = Wrap(draft.Pet + delta, 3); break;
                case 10: draft.FarmType = Wrap(draft.FarmType + delta, FarmTypes.Count); break;
                case 11: draft.SkipIntro = !draft.SkipIntro; break;
            }
        }

        private static int Wrap(int value, int count)
        {
            if (count <= 0)
                return 0;
            value %= count;
            if (value < 0)
                value += count;
            return value;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch, Panel, "Criar personagem");

            if (font != null)
            {
                spriteBatch.DrawString(font,
                    "Nome, aparencia e tipo de fazenda",
                    new Vector2(Panel.X + 32, Panel.Y + 54),
                    new Color(220, 220, 200));
            }

            DrawRow(spriteBatch, NameRect, "Nome: " + draft.FarmerName, activeField == 1);
            DrawRow(spriteBatch, FarmNameRect, "Fazenda: " + draft.FarmName, activeField == 2);
            DrawRow(spriteBatch, FavoriteRect, "Favorito: " + draft.FavoriteThing, activeField == 3);

            DrawPortrait(spriteBatch);
            DrawGenderAndPet(spriteBatch);
            DrawOptions(spriteBatch);
            DrawFarmInfo(spriteBatch);
            DrawFarmButtons(spriteBatch);

            DrawRow(spriteBatch, OkRect, "OK - comecar", true);
            DrawBack(spriteBatch);

            keyboard.Draw(spriteBatch, pixel, font, activeField == 2
                ? "Nome da fazenda"
                : activeField == 3 ? "Coisa favorita" : "Nome do fazendeiro");
        }

        private void DrawPortrait(SpriteBatch batch)
        {
            if (dayBackground != null)
                batch.Draw(dayBackground, Portrait, new Rectangle(0, 0, dayBackground.Width, dayBackground.Height), Color.White);
            else
                batch.Draw(pixel, Portrait, new Color(42, 64, 92));

            batch.Draw(pixel, new Rectangle(Portrait.X, Portrait.Y, Portrait.Width, 4), new Color(126, 170, 92));
            batch.Draw(pixel, new Rectangle(Portrait.X, Portrait.Bottom - 4, Portrait.Width, 4), new Color(126, 170, 92));

            var body = draft.IsMale ? maleBase : femaleBase;
            var dest = new Rectangle(
                Portrait.X + (Portrait.Width - 64) / 2,
                Portrait.Bottom - 136,
                64, 128);

            if (body != null)
            {
                var tint = Blend(Color.White, skinColors[draft.Skin % skinColors.Length], 0.35f);
                batch.Draw(body, dest, new Rectangle(0, 0, 16, 32), tint);
            }

            batch.Draw(pixel,
                new Rectangle(dest.X + 8, dest.Bottom - 12, 48, 8),
                shoeColors[draft.Shoes % shoeColors.Length]);

            if (pantsTexture != null)
            {
                var style = draft.Pants % 20;
                var src = new Rectangle(
                    style % 10 * 192 + (draft.IsMale ? 0 : 96),
                    style / 10 * 688,
                    16, 32);
                batch.Draw(pantsTexture, dest, src,
                    PantsPalette[draft.PantsColor % PantsPalette.Length]);
            }

            if (shirtTexture != null && shirtTexture.Width >= 128)
            {
                var shirt = draft.Shirt % 200;
                var src = new Rectangle(shirt * 8 % 128, shirt * 8 / 128 * 32, 8, 8);
                var shirtDest = new Rectangle(dest.X + 16, dest.Y + 56, 32, 32);
                batch.Draw(shirtTexture, shirtDest, src, Color.White);
                batch.Draw(shirtTexture, shirtDest,
                    new Rectangle(src.X + 128, src.Y, src.Width, src.Height),
                    ShirtPalette[draft.ShirtColor % ShirtPalette.Length]);
            }

            if (accessoryTexture != null && draft.Accessory >= 0)
            {
                var src = new Rectangle(
                    draft.Accessory * 16 % accessoryTexture.Width,
                    draft.Accessory * 16 / accessoryTexture.Width * 32,
                    16, 16);
                batch.Draw(accessoryTexture,
                    new Rectangle(dest.X, dest.Y + 8, 64, 32),
                    src,
                    HairPalette[draft.HairColor % HairPalette.Length]);
            }

            if (hairTexture != null)
            {
                var hair = draft.Hair % 96;
                var src = new Rectangle(
                    hair * 16 % hairTexture.Width,
                    hair * 16 / hairTexture.Width * 96,
                    16, 32);
                var hairDest = dest;
                hairDest.Y += draft.IsMale && hair >= 16 ? -4 : (!draft.IsMale && hair < 16 ? 4 : 0);
                batch.Draw(hairTexture, hairDest, src,
                    HairPalette[draft.HairColor % HairPalette.Length]);
            }
        }

        private static Color Blend(Color a, Color b, float amount)
        {
            return new Color(
                (int)(a.R * (1f - amount) + b.R * amount),
                (int)(a.G * (1f - amount) + b.G * amount),
                (int)(a.B * (1f - amount) + b.B * amount));
        }

        private void DrawGenderAndPet(SpriteBatch batch)
        {
            DrawButton(batch, MaleRect, "M", draft.IsMale);
            DrawButton(batch, FemaleRect, "F", !draft.IsMale);
            DrawPetButton(batch, PetNoneRect, "X", draft.Pet == 0, null);
            DrawPetButton(batch, PetDogRect, "Cao", draft.Pet == 1, new Rectangle(208, 208, 16, 16));
            DrawPetButton(batch, PetCatRect, "Gato", draft.Pet == 2, new Rectangle(160, 208, 16, 16));

            if (font == null)
                return;

            DrawCentered(batch, "Genero", new Rectangle(304, 288, 144, 20));
            DrawCentered(batch, "Animal", new Rectangle(304, 358, 224, 20));
        }

        private void DrawButton(SpriteBatch batch, Rectangle rect, string label, bool selected)
        {
            batch.Draw(pixel, rect, selected ? new Color(96, 142, 72) : new Color(40, 72, 48));
            if (font != null)
                DrawCentered(batch, label, rect);
            if (selected)
                batch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - 4, rect.Width, 4), new Color(255, 228, 150));
        }

        private void DrawPetButton(SpriteBatch batch, Rectangle rect, string label, bool selected, Rectangle? icon)
        {
            DrawButton(batch, rect, icon.HasValue ? string.Empty : label, selected);
            if (icon.HasValue && cursors != null)
            {
                batch.Draw(cursors,
                    new Rectangle(rect.X + (rect.Width - 32) / 2, rect.Y + 4, 32, 32),
                    icon.Value, Color.White);
            }
        }

        private void DrawOptions(SpriteBatch batch)
        {
            if (font == null)
                return;

            DrawCentered(batch, "Aparencia", new Rectangle(500, 214, 464, 18));

            for (var i = 0; i < 12; i++)
            {
                var row = OptionRect(i);
                batch.Draw(pixel, row, new Color(32, 60, 42) * 0.96f);

                var left = new Rectangle(row.X, row.Y, 40, row.Height);
                var right = new Rectangle(row.Right - 40, row.Y, 40, row.Height);
                DrawArrow(batch, left, true);
                DrawArrow(batch, right, false);

                var text = OptionLabel(i) + ": " + OptionValue(i);
                var size = font.MeasureString(text);
                var scale = size.X > row.Width - 88 ? 0.8f : 1f;
                size = font.MeasureString(text) * scale;
                var x = row.X + (row.Width - size.X) / 2f;
                var y = row.Y + (row.Height - size.Y) / 2f;
                batch.DrawString(font, text, new Vector2(x, y), Color.White, 0f,
                    Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        private void DrawArrow(SpriteBatch batch, Rectangle rect, bool left)
        {
            if (cursors == null)
            {
                batch.Draw(pixel, rect, new Color(64, 96, 70));
                return;
            }

            var source = left ? new Rectangle(0, 16, 16, 16) : new Rectangle(528, 0, 16, 16);
            batch.Draw(cursors,
                new Rectangle(rect.X + 4, rect.Y + 1, 32, 32),
                source, Color.White);
        }

        private static string OptionLabel(int index)
        {
            switch (index)
            {
                case 0: return Localization.UIString("Strings\\UI:Character_Skin", "Pele");
                case 1: return Localization.UIString("Strings\\UI:Character_Hair", "Cabelo");
                case 2: return Localization.UIString("Strings\\UI:Character_HairColor", "Cor cabelo");
                case 3: return Localization.UIString("Strings\\UI:Character_Shirt", "Camisa");
                case 4: return Localization.UIString("Strings\\UI:Character_ShirtColor", "Cor camisa");
                case 5: return Localization.UIString("Strings\\UI:Character_Pants", "Calcas");
                case 6: return Localization.UIString("Strings\\UI:Character_PantsColor", "Cor calcas");
                case 7: return Localization.UIString("Strings\\UI:Character_Shoes", "Sapatos");
                case 8: return Localization.UIString("Strings\\UI:Character_Accessory", "Acessorio");
                case 9: return Localization.UIString("Strings\\UI:Character_Animal", "Animal");
                case 10: return Localization.UIString("Strings\\UI:Character_Farm", "Fazenda");
                default: return Localization.UIString("Strings\\UI:Character_SkipIntro", "Pular intro");
            }
        }

        private string OptionValue(int index)
        {
            switch (index)
            {
                case 0: return (draft.Skin + 1).ToString();
                case 1: return (draft.Hair + 1).ToString();
                case 2: return (draft.HairColor + 1).ToString();
                case 3: return (draft.Shirt + 1).ToString();
                case 4: return (draft.ShirtColor + 1).ToString();
                case 5: return (draft.Pants + 1).ToString();
                case 6: return (draft.PantsColor + 1).ToString();
                case 7: return (draft.Shoes + 1).ToString();
                case 8: return draft.Accessory < 0 ? "Nenhum" : (draft.Accessory + 1).ToString();
                case 9: return draft.Pet == 1 ? "Cao" : (draft.Pet == 2 ? "Gato" : "Nenhum");
                case 10: return FarmTypes.Name(draft.FarmType);
                default: return draft.SkipIntro ? "Sim" : "Nao";
            }
        }

        private void DrawFarmInfo(SpriteBatch batch)
        {
            if (font == null)
                return;

            batch.Draw(pixel, FarmInfoRect, new Color(20, 44, 30) * 0.94f);
            batch.Draw(pixel, new Rectangle(FarmInfoRect.X, FarmInfoRect.Y, FarmInfoRect.Width, 3),
                new Color(126, 170, 92));

            var title = "Fazenda: " + FarmTypes.Name(draft.FarmType);
            batch.DrawString(font, title, new Vector2(FarmInfoRect.X + 10, FarmInfoRect.Y + 10),
                new Color(255, 236, 160));

            DrawWrapped(batch, FarmTypes.Description(draft.FarmType),
                FarmInfoRect.X + 10, FarmInfoRect.Y + 38, FarmInfoRect.Width - 20,
                FarmInfoRect.Height - 46);
        }

        private void DrawFarmButtons(SpriteBatch batch)
        {
            if (font != null)
            {
                batch.DrawString(font, "Tipo de fazenda",
                    new Vector2(1008, 20), new Color(255, 236, 160));
            }

            for (var i = 0; i < FarmTypes.Count; i++)
            {
                var rect = FarmButtonRect(i);
                batch.Draw(pixel, rect, new Color(32, 60, 42));

                if (cursors != null)
                {
                    var x = i < 5 ? i * 22 : (i - 5) * 22;
                    var y = i < 5 ? 324 : 345;
                    batch.Draw(cursors, rect,
                        new Rectangle(x, y, 22, 20), Color.White);
                }

                if (i == draft.FarmType)
                {
                    batch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 4), new Color(255, 228, 150));
                    batch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - 4, rect.Width, 4), new Color(255, 228, 150));
                    batch.Draw(pixel, new Rectangle(rect.X, rect.Y, 4, rect.Height), new Color(255, 228, 150));
                    batch.Draw(pixel, new Rectangle(rect.Right - 4, rect.Y, 4, rect.Height), new Color(255, 228, 150));
                }
            }
        }

        private void DrawCentered(SpriteBatch batch, string text, Rectangle area)
        {
            if (font == null || string.IsNullOrEmpty(text))
                return;

            var size = font.MeasureString(text);
            batch.DrawString(font, text,
                new Vector2(area.X + (area.Width - size.X) / 2f, area.Y + (area.Height - size.Y) / 2f),
                Color.White);
        }

        private void DrawWrapped(SpriteBatch batch, string text, float x, float y, float width, float height)
        {
            if (font == null || string.IsNullOrEmpty(text))
                return;

            var words = text.Split(' ');
            var line = string.Empty;
            var lineHeight = font.MeasureString("Ag").Y + 4f;
            var maxLines = (int)(height / lineHeight);
            var lines = 0;

            for (var i = 0; i < words.Length && lines < maxLines; i++)
            {
                var candidate = line.Length == 0 ? words[i] : line + " " + words[i];
                if (line.Length > 0 && font.MeasureString(candidate).X > width)
                {
                    batch.DrawString(font, line, new Vector2(x, y + lines * lineHeight), Color.White);
                    lines++;
                    line = words[i];
                }
                else
                {
                    line = candidate;
                }
            }

            if (lines < maxLines && line.Length > 0)
                batch.DrawString(font, line, new Vector2(x, y + lines * lineHeight), Color.White);
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
    }
}
