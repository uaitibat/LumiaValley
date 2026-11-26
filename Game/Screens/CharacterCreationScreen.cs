using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Tela de criacao de personagem da 1.5.6 reimplementada para o port.
    /// A geometria, a ordem dos elementos e as regioes de sprite seguem o
    /// observado no jogo original; nenhum codigo descompilado e copiado.
    /// </summary>
    public sealed class CharacterCreationScreen : MenuScreen
    {
        private static readonly Rectangle MenuRect = new Rectangle(284, -68, 712, 792);
        private static readonly Rectangle PortraitRect = new Rectangle(388, 52, 128, 192);
        private static readonly Rectangle NameRect = new Rectangle(660, 52, 192, 48);
        private static readonly Rectangle FarmNameRect = new Rectangle(660, 116, 192, 48);
        private static readonly Rectangle FavoriteRect = new Rectangle(660, 180, 192, 48);
        private static readonly Rectangle RandomRect = new Rectangle(332, 52, 40, 40);
        private static readonly Rectangle MaleRect = new Rectangle(380, 260, 64, 64);
        private static readonly Rectangle FemaleRect = new Rectangle(460, 260, 64, 64);
        private static readonly Rectangle DirectionLeftRect = new Rectangle(356, 196, 64, 64);
        private static readonly Rectangle DirectionRightRect = new Rectangle(484, 196, 64, 64);
        private static readonly Rectangle PetLeftRect = new Rectangle(708, 244, 64, 64);
        private static readonly Rectangle PetPortraitRect = new Rectangle(772, 244, 64, 64);
        private static readonly Rectangle PetRightRect = new Rectangle(836, 244, 64, 64);
        private static readonly Rectangle SkipIntroRect = new Rectangle(612, 540, 36, 36);
        private static readonly Rectangle OkRect = new Rectangle(876, 604, 64, 64);
        private static readonly Rectangle AdvancedRect = new Rectangle(204, 628, 80, 80);
        private static readonly Rectangle BackButtonRect = new Rectangle(1034, 615, 198, 81);
        private static readonly Rectangle FarmPanelRect = new Rectangle(992, 40, 120, 652);
        private static readonly Rectangle FarmPageLeftRect = new Rectangle(960, 336, 64, 64);
        private static readonly Rectangle FarmPageRightRect = new Rectangle(1080, 336, 64, 64);

        private static readonly Color[] HairPalette =
        {
            new Color(110, 58, 30), new Color(129, 66, 36), new Color(155, 79, 43),
            new Color(193, 90, 50), new Color(224, 137, 83), new Color(246, 191, 121),
            new Color(255, 222, 170), new Color(148, 118, 92), new Color(94, 73, 54),
            new Color(75, 49, 32), new Color(45, 35, 25), new Color(241, 174, 73)
        };

        private static readonly Color[] EyePalette =
        {
            new Color(96, 64, 40), new Color(128, 88, 56), new Color(160, 116, 76),
            new Color(80, 112, 128), new Color(96, 140, 160), new Color(128, 176, 188),
            new Color(64, 96, 72), new Color(96, 132, 84), new Color(128, 164, 104),
            new Color(112, 72, 120), new Color(148, 104, 148), new Color(180, 148, 180)
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
        private readonly Random random = new Random();
        private TitleScreen backdrop;
        private Texture2D menuTiles;
        private Texture2D cursors;
        private Texture2D cursors2;
        private Texture2D titleButtons;
        private Texture2D dayBackground;
        private Texture2D maleBase;
        private Texture2D femaleBase;
        private Texture2D hairTexture;
        private Texture2D shirtTexture;
        private Texture2D pantsTexture;
        private Texture2D accessoryTexture;
        private Color[] skinColors;
        private Color[] shoeColors;

        private int activeField;
        private double elapsedMs;
        private int farmHintMs;
        private bool advanced;

        public CharacterCreationScreen()
        {
            // O original comeca com os tres campos vazios e o OK desabilitado.
            draft.FarmerName = string.Empty;
            draft.FarmName = string.Empty;
            draft.FavoriteThing = string.Empty;
            draft.IsMale = true;
            draft.Skin = 0;
            draft.Hair = 0;
            draft.HairColor = 0;
            draft.EyeColor = 0;
            draft.Shirt = 0;
            draft.ShirtColor = 0;
            draft.Pants = 0;
            draft.PantsColor = 0;
            draft.Shoes = 2;
            draft.Accessory = 0;
            draft.Pet = 1;
            draft.Facing = 0;
            draft.FarmType = 0;
            draft.SkipIntro = false;
        }

        public override void Load(ContentManager content, GraphicsDevice device)
        {
            base.Load(content, device);

            // A criacao e um subMenu sobre a home: reaproveitamos o ceu e o
            // logo ja desenhados pelo proprio port.
            backdrop = new TitleScreen(true);
            backdrop.Load(content, device);

            menuTiles = LoadTexture(content, "Maps/MenuTiles");
            cursors = LoadTexture(content, "LooseSprites/Cursors.pt-BR", "LooseSprites/Cursors");
            cursors2 = LoadTexture(content, "LooseSprites/Cursors2");
            titleButtons = LoadTexture(content, "Minigames/TitleButtons.pt-BR", "Minigames/TitleButtons");
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

        private bool CanStart
        {
            get
            {
                return !string.IsNullOrWhiteSpace(draft.FarmerName)
                    && !string.IsNullOrWhiteSpace(draft.FarmName)
                    && !string.IsNullOrWhiteSpace(draft.FavoriteThing);
            }
        }

        public override IScreen Update(GameTime gameTime, Vector2? tap)
        {
            elapsedMs += Math.Min(gameTime.ElapsedGameTime.TotalMilliseconds, 50);
            if (farmHintMs > 0)
                farmHintMs -= (int)Math.Min(gameTime.ElapsedGameTime.TotalMilliseconds, 50);

            if (!tap.HasValue)
                return this;

            var point = tap.Value;

            if (keyboard.Visible)
            {
                if (Contains(keyboard.Bounds, point))
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

            if (Contains(BackButtonRect, point))
                return Home();

            if (Contains(RandomRect, point))
            {
                Randomize();
                return this;
            }

            if (Contains(NameRect, point)) { activeField = 1; keyboard.Show(); return this; }
            if (Contains(FarmNameRect, point)) { activeField = 2; keyboard.Show(); return this; }
            if (Contains(FavoriteRect, point)) { activeField = 3; keyboard.Show(); return this; }

            if (Contains(MaleRect, point)) { draft.IsMale = true; return this; }
            if (Contains(FemaleRect, point)) { draft.IsMale = false; return this; }

            if (Contains(DirectionLeftRect, point)) { draft.Facing = Wrap(draft.Facing - 1, 4); return this; }
            if (Contains(DirectionRightRect, point)) { draft.Facing = Wrap(draft.Facing + 1, 4); return this; }

            if (Contains(PetLeftRect, point)) { draft.Pet = draft.Pet == 1 ? 2 : 1; return this; }
            if (Contains(PetRightRect, point)) { draft.Pet = draft.Pet == 1 ? 2 : 1; return this; }

            for (var row = 0; row < 5; row++)
            {
                if (Contains(AppearanceArrowRect(row, true), point)) { Adjust(row, -1); return this; }
                if (Contains(AppearanceArrowRect(row, false), point)) { Adjust(row, 1); return this; }
            }

            if (HandleColorPickerTap(point))
                return this;

            if (Contains(SkipIntroRect, point)) { draft.SkipIntro = !draft.SkipIntro; return this; }

            for (var i = 0; i < FarmTypes.Count; i++)
            {
                if (!Contains(FarmButtonRect(i), point))
                    continue;
                draft.FarmType = i;
                farmHintMs = 2600;
                return this;
            }

            if (Contains(FarmPageLeftRect, point))
            {
                draft.FarmType = Wrap(draft.FarmType - 1, FarmTypes.Count);
                farmHintMs = 2600;
                return this;
            }
            if (Contains(FarmPageRightRect, point))
            {
                draft.FarmType = Wrap(draft.FarmType + 1, FarmTypes.Count);
                farmHintMs = 2600;
                return this;
            }

            if (Contains(AdvancedRect, point)) { advanced = !advanced; return this; }

            if (advanced)
            {
                for (var row = 0; row < 3; row++)
                {
                    var r = AdvancedRowRect(row);
                    if (!Contains(r, point))
                        continue;
                    var delta = Contains(new Rectangle(r.Right - 42, r.Y, 42, r.Height), point) ? 1 : -1;
                    AdjustAdvanced(row, delta);
                    return this;
                }
            }

            if (Contains(OkRect, point) && CanStart)
            {
                draft.FarmerName = draft.FarmerName.Trim();
                draft.FarmName = draft.FarmName.Trim();
                draft.FavoriteThing = draft.FavoriteThing.Trim();
                SaveStore.Create(draft);
                return new FarmScreen(draft);
            }

            return this;
        }

        private static Rectangle FarmButtonRect(int index)
        {
            return new Rectangle(1008, 60 + index * 88, 88, 80);
        }

        private static Rectangle AppearanceArrowRect(int row, bool left)
        {
            return new Rectangle(left ? 336 : 484, 324 + row * 68, 64, 64);
        }

        private static Rectangle AdvancedRowRect(int row)
        {
            return new Rectangle(188, 472 + row * 36, 84, 30);
        }

        private void Randomize()
        {
            draft.IsMale = random.Next(2) == 0;
            draft.Skin = random.Next(skinColors.Length);
            draft.Hair = random.Next(96);
            draft.HairColor = random.Next(HairPalette.Length);
            draft.EyeColor = random.Next(EyePalette.Length);
            draft.Shirt = random.Next(20);
            draft.ShirtColor = random.Next(ShirtPalette.Length);
            draft.Pants = random.Next(20);
            draft.PantsColor = random.Next(PantsPalette.Length);
            draft.Shoes = random.Next(shoeColors.Length);
            draft.Accessory = random.Next(20);
            draft.Pet = random.Next(1, 3);
            draft.Facing = random.Next(4);
        }

        private void Adjust(int row, int delta)
        {
            switch (row)
            {
                case 0: draft.Skin = Wrap(draft.Skin + delta, skinColors.Length); break;
                case 1: draft.Hair = Wrap(draft.Hair + delta, 96); break;
                case 2: draft.Shirt = Wrap(draft.Shirt + delta, 20); break;
                case 3: draft.Pants = Wrap(draft.Pants + delta, 20); break;
                case 4:
                    if (draft.Accessory < 0 && delta > 0) draft.Accessory = 0;
                    else draft.Accessory = Wrap(draft.Accessory + delta, 20);
                    break;
            }
        }

        private void AdjustAdvanced(int row, int delta)
        {
            if (row == 0) draft.ShirtColor = Wrap(draft.ShirtColor + delta, ShirtPalette.Length);
            else if (row == 1) draft.Shoes = Wrap(draft.Shoes + delta, shoeColors.Length);
            else draft.EyeColor = Wrap(draft.EyeColor + delta, EyePalette.Length);
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

        private bool HandleColorPickerTap(Vector2 point)
        {
            for (var picker = 0; picker < 3; picker++)
            {
                var y = 324 + picker * 68;
                for (var bar = 0; bar < 3; bar++)
                {
                    var rect = new Rectangle(708, y + bar * 20, 128, 20);
                    if (!Contains(rect, point))
                        continue;

                    var t = (point.X - rect.X) / (float)rect.Width;
                    if (t < 0f) t = 0f;
                    if (t > 1f) t = 1f;

                    Color current;
                    if (picker == 0) current = EyePalette[draft.EyeColor % EyePalette.Length];
                    else if (picker == 1) current = HairPalette[draft.HairColor % HairPalette.Length];
                    else current = PantsPalette[draft.PantsColor % PantsPalette.Length];

                    float h, s, v;
                    RgbToHsv(current, out h, out s, out v);
                    if (bar == 0) h = t * 360f;
                    else if (bar == 1) s = t;
                    else v = t * 255f;

                    var target = HsvToRgb(h, s, v);
                    if (picker == 0) draft.EyeColor = Nearest(target, EyePalette);
                    else if (picker == 1) draft.HairColor = Nearest(target, HairPalette);
                    else draft.PantsColor = Nearest(target, PantsPalette);
                    return true;
                }
            }

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (backdrop != null)
                backdrop.DrawBackdrop(spriteBatch);

            DrawMenuBox(spriteBatch, MenuRect);
            DrawPortrait(spriteBatch);
            DrawGenderAndPet(spriteBatch);
            DrawAppearance(spriteBatch);
            DrawColorPickers(spriteBatch);
            DrawTextBoxes(spriteBatch);
            DrawSkipIntro(spriteBatch);
            DrawFarmPanel(spriteBatch);
            DrawOk(spriteBatch);
            DrawAdvanced(spriteBatch);
            DrawBackButton(spriteBatch);
            DrawFarmHint(spriteBatch);

            keyboard.Draw(spriteBatch, pixel, font, activeField == 2
                ? "Nome da fazenda"
                : activeField == 3 ? "Coisa favorita" : "Nome do fazendeiro");
        }

        private void DrawMenuBox(SpriteBatch batch, Rectangle area)
        {
            if (menuTiles == null || menuTiles.Width < 256 || menuTiles.Height < 256)
            {
                batch.Draw(pixel, area, new Color(24, 48, 32));
                return;
            }

            const int b = 64;
            batch.Draw(menuTiles, new Rectangle(area.X, area.Y, b, b), new Rectangle(0, 0, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X + b, area.Y, area.Width - b * 2, b), new Rectangle(128, 0, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.Right - b, area.Y, b, b), new Rectangle(192, 0, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X, area.Y + b, b, area.Height - b * 2), new Rectangle(0, 128, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X + b, area.Y + b, area.Width - b * 2, area.Height - b * 2), new Rectangle(64, 128, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.Right - b, area.Y + b, b, area.Height - b * 2), new Rectangle(192, 128, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X, area.Bottom - b, b, b), new Rectangle(0, 192, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X + b, area.Bottom - b, area.Width - b * 2, b), new Rectangle(128, 192, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.Right - b, area.Bottom - b, b, b), new Rectangle(192, 192, b, b), Color.White);
        }

        private void DrawSmallBox(SpriteBatch batch, Rectangle area)
        {
            if (menuTiles == null || menuTiles.Width < 60 || menuTiles.Height < 316)
            {
                batch.Draw(pixel, area, new Color(24, 48, 32));
                return;
            }

            const int b = 20;
            var src = new Rectangle(0, 256, 60, 60);
            batch.Draw(menuTiles, new Rectangle(area.X, area.Y, b, b), new Rectangle(src.X, src.Y, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X + b, area.Y, area.Width - b * 2, b), new Rectangle(src.X + b, src.Y, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.Right - b, area.Y, b, b), new Rectangle(src.X + b * 2, src.Y, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X, area.Y + b, b, area.Height - b * 2), new Rectangle(src.X, src.Y + b, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X + b, area.Y + b, area.Width - b * 2, area.Height - b * 2), new Rectangle(src.X + b, src.Y + b, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.Right - b, area.Y + b, b, area.Height - b * 2), new Rectangle(src.X + b * 2, src.Y + b, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X, area.Bottom - b, b, b), new Rectangle(src.X, src.Y + b * 2, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.X + b, area.Bottom - b, area.Width - b * 2, b), new Rectangle(src.X + b, src.Y + b * 2, b, b), Color.White);
            batch.Draw(menuTiles, new Rectangle(area.Right - b, area.Bottom - b, b, b), new Rectangle(src.X + b * 2, src.Y + b * 2, b, b), Color.White);
        }

        private void DrawPortrait(SpriteBatch batch)
        {
            if (dayBackground != null)
                batch.Draw(dayBackground, PortraitRect, new Rectangle(0, 0, dayBackground.Width, dayBackground.Height), Color.White);
            else
                batch.Draw(pixel, PortraitRect, new Color(42, 64, 92));

            DrawArrow(batch, DirectionLeftRect, true);
            DrawArrow(batch, DirectionRightRect, false);

            if (cursors != null)
                batch.Draw(cursors, RandomRect, new Rectangle(381, 361, 10, 10), Color.White);

            var dest = new Rectangle(PortraitRect.X + 32, PortraitRect.Bottom - 160, 64, 128);
            var faceY = draft.Facing * 32;
            var body = draft.IsMale ? maleBase : femaleBase;
            var skin = skinColors[draft.Skin % skinColors.Length];

            if (body != null)
                batch.Draw(body, dest, new Rectangle(0, faceY, 16, 32), skin);

            if (pantsTexture != null)
            {
                var style = draft.Pants % 20;
                var src = new Rectangle(
                    style % 10 * 192 + (draft.IsMale ? 0 : 96),
                    style / 10 * 688 + faceY,
                    16, 32);
                batch.Draw(pantsTexture, dest, src, PantsPalette[draft.PantsColor % PantsPalette.Length]);
            }

            if (shirtTexture != null && draft.Shirt < 20)
            {
                var src = new Rectangle(
                    draft.Shirt * 8 % 128,
                    draft.Shirt * 8 / 128 * 32 + faceY,
                    8, 8);
                batch.Draw(shirtTexture, new Rectangle(dest.X + 16, dest.Y + 56, 32, 32), src, Color.White);
                batch.Draw(shirtTexture, new Rectangle(dest.X + 16, dest.Y + 56, 32, 32),
                    new Rectangle(src.X + 128, src.Y, src.Width, src.Height),
                    ShirtPalette[draft.ShirtColor % ShirtPalette.Length]);
            }

            if (accessoryTexture != null && draft.Accessory >= 0)
            {
                var src = new Rectangle(
                    draft.Accessory * 16 % accessoryTexture.Width,
                    draft.Accessory * 16 / accessoryTexture.Width * 32,
                    16, 16);
                batch.Draw(accessoryTexture, new Rectangle(dest.X, dest.Y + 4, 64, 64), src,
                    draft.Accessory < 6 ? HairPalette[draft.HairColor % HairPalette.Length] : Color.White);
            }

            if (hairTexture != null)
            {
                var hair = draft.Hair % 96;
                var src = new Rectangle(
                    hair * 16 % hairTexture.Width,
                    hair * 16 / hairTexture.Width * 96 + faceY,
                    16, 32);
                batch.Draw(hairTexture, dest, src, HairPalette[draft.HairColor % HairPalette.Length]);
            }

            batch.Draw(pixel,
                new Rectangle(dest.X + 8, dest.Bottom - 12, 48, 8),
                shoeColors[draft.Shoes % shoeColors.Length]);
        }

        private void DrawGenderAndPet(SpriteBatch batch)
        {
            if (cursors != null)
            {
                batch.Draw(cursors, MaleRect, new Rectangle(128, 192, 16, 16), Color.White);
                batch.Draw(cursors, FemaleRect, new Rectangle(144, 192, 16, 16), Color.White);
                if (draft.IsMale)
                    batch.Draw(cursors, MaleRect, new Rectangle(64, 192, 64, 64), Color.White);
                else
                    batch.Draw(cursors, FemaleRect, new Rectangle(64, 192, 64, 64), Color.White);
            }
            else
            {
                batch.Draw(pixel, MaleRect, draft.IsMale ? new Color(96, 142, 72) : new Color(48, 82, 54));
                batch.Draw(pixel, FemaleRect, !draft.IsMale ? new Color(96, 142, 72) : new Color(48, 82, 54));
            }

            DrawArrow(batch, PetLeftRect, true);
            DrawArrow(batch, PetRightRect, false);

            if (cursors != null)
            {
                var x = draft.Pet == 1 ? 208 : 160;
                batch.Draw(cursors, PetPortraitRect, new Rectangle(x, 208, 16, 16), Color.White);
                batch.Draw(cursors, PetPortraitRect, new Rectangle(64, 192, 64, 64), Color.White);
            }
            else
            {
                batch.Draw(pixel, PetPortraitRect, new Color(48, 82, 54));
            }

            ShadowText(batch, Localization.UIString("Strings\\UI:Character_Animal", "Animal"),
                540, 252, Color.White, 1f);
        }

        private void DrawAppearance(SpriteBatch batch)
        {
            for (var row = 0; row < 5; row++)
            {
                DrawArrow(batch, AppearanceArrowRect(row, true), true);
                DrawArrow(batch, AppearanceArrowRect(row, false), false);
            }

            ShadowText(batch, AppearanceLabel(0) + ": " + (draft.Skin + 1), 418, 340, Color.White, 1f);
            ShadowText(batch, AppearanceLabel(1) + ": " + (draft.Hair + 1), 418, 408, Color.White, 1f);
            ShadowText(batch, AppearanceLabel(2) + ": " + (draft.Shirt + 1), 418, 476, Color.White, 1f);
            ShadowText(batch, AppearanceLabel(3) + ": " + (draft.Pants + 1), 418, 544, Color.White, 1f);
            var accessory = draft.Accessory < 0
                ? Localization.UIString("Strings\\UI:Character_None", "Nenhum")
                : (draft.Accessory + 1).ToString();
            ShadowText(batch, AppearanceLabel(4) + ": " + accessory, 418, 612, Color.White, 1f);

            ShadowText(batch, Localization.UIString("Strings\\UI:Character_EyeColor", "Olhos"),
                556, 340, Color.White, 1f);
            ShadowText(batch, Localization.UIString("Strings\\UI:Character_HairColor", "Cor cabelo"),
                556, 408, Color.White, 1f);
            ShadowText(batch, Localization.UIString("Strings\\UI:Character_PantsColor", "Cor calças"),
                556, 476, Color.White, 1f);
        }

        private static string AppearanceLabel(int row)
        {
            switch (row)
            {
                case 0: return Localization.UIString("Strings\\UI:Character_Skin", "Pele");
                case 1: return Localization.UIString("Strings\\UI:Character_Hair", "Cabelo");
                case 2: return Localization.UIString("Strings\\UI:Character_Shirt", "Camisa");
                case 3: return Localization.UIString("Strings\\UI:Character_Pants", "Calças");
                default: return Localization.UIString("Strings\\UI:Character_Accessory", "Acessório");
            }
        }

        private void DrawArrow(SpriteBatch batch, Rectangle rect, bool left)
        {
            if (cursors == null)
            {
                batch.Draw(pixel, rect, new Color(48, 82, 54));
                if (font != null)
                    batch.DrawString(font, left ? "<" : ">", new Vector2(rect.X + 22, rect.Y + 20), Color.White);
                return;
            }

            // tile 44 = seta para a esquerda, tile 33 = seta para a direita.
            var source = left ? new Rectangle(0, 256, 64, 64) : new Rectangle(0, 192, 64, 64);
            batch.Draw(cursors, rect, source, Color.White);
        }

        private void DrawColorPickers(SpriteBatch batch)
        {
            DrawColorPicker(batch, 0, 324, EyePalette[draft.EyeColor % EyePalette.Length], draft.EyeColor);
            DrawColorPicker(batch, 1, 392, HairPalette[draft.HairColor % HairPalette.Length], draft.HairColor);
            DrawColorPicker(batch, 2, 460, PantsPalette[draft.PantsColor % PantsPalette.Length], draft.PantsColor);
        }

        private void DrawColorPicker(SpriteBatch batch, int kind, int y, Color color, int value)
        {
            var x = 708;
            float h, s, v;
            RgbToHsv(color, out h, out s, out v);

            var segments = 24;
            for (var i = 0; i < segments; i++)
            {
                var c = HsvToRgb(i / (float)segments * 360f, 0.9f, 0.9f);
                var rx = x + i * 128 / segments;
                var rw = (i + 1) * 128 / segments - rx;
                batch.Draw(pixel, new Rectangle(rx, y, rw, 4), c);
            }

            for (var i = 0; i < 8; i++)
            {
                var t = i / 7f;
                var grey = (int)(v * t);
                batch.Draw(pixel, new Rectangle(x + i * 16, y + 20, 16, 4),
                    new Color(grey, grey, grey));
                batch.Draw(pixel, new Rectangle(x + i * 16, y + 40, 16, 4),
                    Mix(color, new Color(0, 0, 0), 1f - t));
            }

            if (cursors != null)
            {
                batch.Draw(cursors,
                    new Vector2(x + h / 360f * 128f, y + 2),
                    new Rectangle(64, 256, 32, 32),
                    Color.White, 0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0f);
                batch.Draw(cursors,
                    new Vector2(x + s * 128f, y + 22),
                    new Rectangle(64, 256, 32, 32),
                    Color.White, 0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0f);
                batch.Draw(cursors,
                    new Vector2(x + v / 255f * 128f, y + 42),
                    new Rectangle(64, 256, 32, 32),
                    Color.White, 0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0f);
            }

            if (font != null)
                batch.DrawString(font, (value + 1).ToString(), new Vector2(x + 136, y + 20), Color.White);

            if (kind < 0)
                return;
        }

        private void DrawTextBoxes(SpriteBatch batch)
        {
            DrawTextBox(batch, NameRect, draft.FarmerName,
                "Strings\\UI:Character_Name", "Nome", 548, 60, 1);
            DrawTextBox(batch, FarmNameRect, draft.FarmName,
                "Strings\\UI:Character_Farm", "Fazenda", 540, 116, 2);
            DrawTextBox(batch, FavoriteRect, draft.FavoriteThing,
                "Strings\\UI:Character_FavoriteThing", "Coisa favorita", 548, 180, 3);

            var suffix = Localization.UIString("Strings\\UI:Character_FarmNameSuffix", string.Empty);
            if (!string.IsNullOrEmpty(suffix))
                ShadowText(batch, suffix, 860, 128, Color.White, 1f);
        }

        private void DrawTextBox(SpriteBatch batch, Rectangle rect, string value,
            string labelKey, string labelFallback, int labelX, int labelY, int field)
        {
            var selected = activeField == field && keyboard.Visible;
            if (textBox != null && textBox.Width >= 192 && textBox.Height >= 48)
            {
                batch.Draw(textBox, new Rectangle(rect.X, rect.Y, 16, 48), new Rectangle(0, 0, 16, 48), Color.White);
                batch.Draw(textBox, new Rectangle(rect.X + 16, rect.Y, 160, 48), new Rectangle(16, 0, 4, 48), Color.White);
                batch.Draw(textBox, new Rectangle(rect.Right - 16, rect.Y, 16, 48), new Rectangle(176, 0, 16, 48), Color.White);
            }
            else
            {
                batch.Draw(pixel, rect, new Color(28, 48, 34));
            }

            if (selected && font != null && (int)(elapsedMs / 500) % 2 == 0)
            {
                var caretX = rect.X + 16 + font.MeasureString(value ?? string.Empty).X;
                batch.Draw(pixel, new Rectangle((int)caretX, rect.Y + 8, 4, 32), new Color(255, 236, 160));
            }

            if (font != null && !string.IsNullOrEmpty(value))
                batch.DrawString(font, value, new Vector2(rect.X + 16, rect.Y + 12), Color.White);

            var empty = string.IsNullOrWhiteSpace(value);
            var labelColor = empty ? Color.Red : Color.White;
            ShadowText(batch, Localization.UIString(labelKey, labelFallback), labelX, labelY, labelColor, 1f);
        }

        private void DrawSkipIntro(SpriteBatch batch)
        {
            if (cursors != null)
            {
                var src = draft.SkipIntro ? new Rectangle(236, 425, 9, 9) : new Rectangle(227, 425, 9, 9);
                batch.Draw(cursors, SkipIntroRect, src, Color.White);
            }
            else
            {
                batch.Draw(pixel, SkipIntroRect, draft.SkipIntro ? new Color(96, 142, 72) : new Color(48, 82, 54));
            }

            ShadowText(batch, Localization.UIString("Strings\\UI:Character_SkipIntro", "Pular intro"),
                SkipIntroRect.Right + 8, SkipIntroRect.Y + 8, Color.White, 1f);
        }

        private void DrawFarmPanel(SpriteBatch batch)
        {
            DrawSmallBox(batch, FarmPanelRect);

            for (var i = 0; i < FarmTypes.Count; i++)
            {
                var rect = FarmButtonRect(i);
                if (cursors != null)
                {
                    var x = i < 5 ? i * 22 : (i - 5) * 22;
                    var y = i < 5 ? 324 : 345;
                    batch.Draw(cursors, rect, new Rectangle(x, y, 22, 20), Color.White);
                }
                else
                {
                    batch.Draw(pixel, rect, new Color(48, 82, 54));
                }

                if (i == draft.FarmType)
                    DrawSelectionFrame(batch, new Rectangle(rect.X, rect.Y - 4, rect.Width, rect.Height + 8));
            }

            DrawArrow(batch, FarmPageLeftRect, true);
            DrawArrow(batch, FarmPageRightRect, false);
        }

        private void DrawSelectionFrame(SpriteBatch batch, Rectangle area)
        {
            if (cursors == null)
            {
                batch.Draw(pixel, new Rectangle(area.X, area.Y, area.Width, 4), new Color(255, 228, 150));
                batch.Draw(pixel, new Rectangle(area.X, area.Bottom - 4, area.Width, 4), new Color(255, 228, 150));
                batch.Draw(pixel, new Rectangle(area.X, area.Y, 4, area.Height), new Color(255, 228, 150));
                batch.Draw(pixel, new Rectangle(area.Right - 4, area.Y, 4, area.Height), new Color(255, 228, 150));
                return;
            }

            var src = new Rectangle(375, 357, 3, 3);
            batch.Draw(cursors, new Rectangle(area.X, area.Y, 12, 12), new Rectangle(src.X, src.Y, 3, 3), Color.White);
            batch.Draw(cursors, new Rectangle(area.Right - 12, area.Y, 12, 12), new Rectangle(src.X + 2, src.Y, 3, 3), Color.White);
            batch.Draw(cursors, new Rectangle(area.X, area.Bottom - 12, 12, 12), new Rectangle(src.X, src.Y + 2, 3, 3), Color.White);
            batch.Draw(cursors, new Rectangle(area.Right - 12, area.Bottom - 12, 12, 12), new Rectangle(src.X + 2, src.Y + 2, 3, 3), Color.White);
            batch.Draw(cursors, new Rectangle(area.X + 12, area.Y, area.Width - 24, 12), new Rectangle(src.X + 1, src.Y, 3, 3), Color.White);
            batch.Draw(cursors, new Rectangle(area.X + 12, area.Bottom - 12, area.Width - 24, 12), new Rectangle(src.X + 1, src.Y + 2, 3, 3), Color.White);
            batch.Draw(cursors, new Rectangle(area.X, area.Y + 12, 12, area.Height - 24), new Rectangle(src.X, src.Y + 1, 3, 3), Color.White);
            batch.Draw(cursors, new Rectangle(area.Right - 12, area.Y + 12, 12, area.Height - 24), new Rectangle(src.X + 2, src.Y + 1, 3, 3), Color.White);
        }

        private void DrawOk(SpriteBatch batch)
        {
            if (cursors != null)
                batch.Draw(cursors, OkRect, new Rectangle(384, 256, 64, 64), Color.White);
            else
                batch.Draw(pixel, OkRect, new Color(96, 142, 72));

            if (!CanStart)
                batch.Draw(pixel, OkRect, Color.Black * 0.5f);
        }

        private void DrawAdvanced(SpriteBatch batch)
        {
            if (cursors2 != null)
                batch.Draw(cursors2, AdvancedRect, new Rectangle(154, 154, 20, 20), Color.White);
            else
                batch.Draw(pixel, AdvancedRect, new Color(48, 82, 54));

            if (!advanced)
                return;

            DrawSmallBox(batch, new Rectangle(180, 452, 100, 164));
            ShadowText(batch, "Avançado", 194, 458, new Color(255, 236, 160), 1f);

            for (var row = 0; row < 3; row++)
            {
                var rect = AdvancedRowRect(row);
                batch.Draw(pixel, rect, new Color(32, 60, 42));
                batch.Draw(pixel, new Rectangle(rect.X + 4, rect.Y + 12, 12, 8), new Color(126, 170, 92));
                batch.Draw(pixel, new Rectangle(rect.Right - 16, rect.Y + 12, 12, 8), new Color(126, 170, 92));

                string text;
                if (row == 0) text = "Camisa " + (draft.ShirtColor + 1);
                else if (row == 1) text = "Sapatos " + (draft.Shoes + 1);
                else text = "Olhos " + (draft.EyeColor + 1);
                ShadowText(batch, text, rect.X + 20, rect.Y + 6, Color.White, 0.8f);
            }
        }

        private void DrawBackButton(SpriteBatch batch)
        {
            if (titleButtons != null)
                batch.Draw(titleButtons, BackButtonRect, new Rectangle(296, 252, 66, 27), Color.White);
            else
                DrawRow(batch, BackButtonRect, Localization.CS("StringsFromCSFiles:TitleMenu.cs.11739", "Voltar"), false);
        }

        private void DrawFarmHint(SpriteBatch batch)
        {
            if (farmHintMs <= 0)
                return;

            var area = new Rectangle(300, 636, 540, 72);
            batch.Draw(pixel, area, Color.Black * 0.82f);
            batch.Draw(pixel, new Rectangle(area.X, area.Y, area.Width, 3), new Color(126, 170, 92));
            ShadowText(batch, FarmTypes.Name(draft.FarmType), area.X + 10, area.Y + 8,
                new Color(255, 236, 160), 1f);
            DrawWrapped(batch, FarmTypes.Description(draft.FarmType),
                area.X + 10, area.Y + 30, area.Width - 20, area.Height - 34);
        }

        private void ShadowText(SpriteBatch batch, string text, float x, float y, Color color, float scale)
        {
            if (font == null || string.IsNullOrEmpty(text))
                return;
            batch.DrawString(font, text, new Vector2(x + 2, y + 2), Color.Black * 0.8f, 0f,
                Vector2.Zero, scale, SpriteEffects.None, 0f);
            batch.DrawString(font, text, new Vector2(x, y), color, 0f,
                Vector2.Zero, scale, SpriteEffects.None, 0f);
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
                    ShadowText(batch, line, x, y + lines * lineHeight, Color.White, 1f);
                    lines++;
                    line = words[i];
                }
                else
                {
                    line = candidate;
                }
            }

            if (lines < maxLines && line.Length > 0)
                ShadowText(batch, line, x, y + lines * lineHeight, Color.White, 1f);
        }

        private static Color Mix(Color a, Color b, float amount)
        {
            if (amount < 0f) amount = 0f;
            if (amount > 1f) amount = 1f;
            return new Color(
                (int)(a.R * (1f - amount) + b.R * amount),
                (int)(a.G * (1f - amount) + b.G * amount),
                (int)(a.B * (1f - amount) + b.B * amount));
        }

        private static void RgbToHsv(Color color, out float h, out float s, out float v)
        {
            var r = color.R / 255f;
            var g = color.G / 255f;
            var b = color.B / 255f;
            var min = Math.Min(Math.Min(r, g), b);
            var max = Math.Max(Math.Max(r, g), b);
            var delta = max - min;
            v = max;
            if (delta <= 0f)
            {
                h = 0f;
                s = 0f;
                return;
            }

            s = delta / max;
            if (r >= max) h = (g - b) / delta;
            else if (g >= max) h = 2f + (b - r) / delta;
            else h = 4f + (r - g) / delta;
            h *= 60f;
            if (h < 0f) h += 360f;
        }

        private static Color HsvToRgb(float h, float s, float v)
        {
            if (s <= 0f)
            {
                var grey = (int)(v * 255f);
                return new Color(grey, grey, grey);
            }

            while (h >= 360f) h -= 360f;
            while (h < 0f) h += 360f;

            var sector = (int)(h / 60f);
            var f = h / 60f - sector;
            var p = v * (1f - s);
            var q = v * (1f - s * f);
            var t = v * (1f - s * (1f - f));

            float r, g, b;
            switch (sector)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                default: r = v; g = p; b = q; break;
            }

            return new Color((int)(r * 255f), (int)(g * 255f), (int)(b * 255f));
        }

        private static int Nearest(Color target, Color[] palette)
        {
            var best = 0;
            var bestDistance = int.MaxValue;
            for (var i = 0; i < palette.Length; i++)
            {
                var dr = target.R - palette[i].R;
                var dg = target.G - palette[i].G;
                var db = target.B - palette[i].B;
                var distance = dr * dr + dg * dg + db * db;
                if (distance >= bestDistance)
                    continue;
                bestDistance = distance;
                best = i;
            }
            return best;
        }

        private static Texture2D LoadTexture(ContentManager content, params string[] assets)
        {
            foreach (var asset in assets)
            {
                try
                {
                    var texture = content.Load<Texture2D>(asset);
                    if (texture != null)
                        return texture;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Asset indisponivel " + asset + ": " + ex.Message);
                }
            }
            return null;
        }
    }
}
