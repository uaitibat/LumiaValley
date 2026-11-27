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
        // O menu original tem 792px de altura e comeca em y=-68. Escalamos
        // todo o conteudo para caber nos 720px virtuais sem cortar bordas.
        private const float UiScale = 0.8888889f;
        private const float UiOffsetY = 68.444444f;

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
        // Deslocado para a direita porque a transformacao de UiScale reduz
        // everything em 0.8889: em coordenadas fisicas fica perto do canto.
        private static readonly Rectangle BackButtonRect = new Rectangle(1216, 620, 200, 82);
        private static readonly Rectangle FarmPanelRect = new Rectangle(992, 40, 120, 652);

        // Cores do original 1.5.6 (Game1.textColor e Utility.drawTextWithShadow).
        private new static readonly Color TextColor = new Color(34, 17, 34);
        private static readonly Color TextShadowColor = new Color(221, 148, 84);
        private static readonly Color SkyColor = new Color(64, 136, 248);

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

        // hairstyles.xnb tem 56 penteados (7 fileiras de 8). HairData mapeia os
        // seguintes para hairstyles2.xnb na mesma grade.
        private const int HairstylesPerSheet = 56;
        private const int HairstyleCount = HairstylesPerSheet * 2;

        private readonly SaveSlot draft;
        private readonly TouchKeyboard keyboard = new TouchKeyboard();
        private readonly Random random = new Random();
        private new Texture2D menuTiles;
        private Texture2D cursors;
        private Texture2D cursors2;
        private Texture2D titleButtons;
        private Texture2D clouds;
        private Texture2D dayBackground;
        private Texture2D maleBase;
        private Texture2D femaleBase;
        private Texture2D hairTexture;
        private Texture2D hairTexture2;
        private Texture2D shirtTexture;
        private Texture2D pantsTexture;
        private Texture2D accessoryTexture;
        private Color[] skinColors;
        private Color[] shoeColors;
        private Texture2D skinTexture;
        private Texture2D shoeTexture;
        private Texture2D recoloredBase;
        private int recoloredSkin = -1;
        private bool recoloredMale;

        // Os ColorPicker do original guardam tres barras com valores 0..100.
        // 0 = olhos, 1 = cabelo, 2 = calca; cada bloco tem hue, saturacao e valor.
        private readonly int[] pickerValues = new int[9];

        private int activeField;
        private double elapsedMs;
        private int farmHintMs;
        private int warningMs;

        public CharacterCreationScreen() : this(null)
        {
        }

        /// <summary>Usado ao voltar das opcoes avancadas, preservando o rascunho.</summary>
        public CharacterCreationScreen(SaveSlot existing)
        {
            draft = existing ?? new SaveSlot();
            if (existing == null)
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
                draft.Accessory = -1;
                draft.Pet = 1;
                draft.Facing = 2;
                draft.FarmType = 0;
                draft.SkipIntro = false;
            }
            SyncPickers();
        }

        public override Matrix Transform
        {
            get
            {
                return Matrix.CreateScale(UiScale) *
                    Matrix.CreateTranslation(0f, UiOffsetY, 0f);
            }
        }

        public override Vector2 FromVirtual(Vector2 point)
        {
            return new Vector2(point.X / UiScale, (point.Y - UiOffsetY) / UiScale);
        }

        public override void Load(ContentManager content, GraphicsDevice device)
        {
            base.Load(content, device);

            menuTiles = LoadTexture(content, "Maps/MenuTiles");
            cursors = LoadTexture(content, "LooseSprites/Cursors.pt-BR", "LooseSprites/Cursors");
            cursors2 = LoadTexture(content, "LooseSprites/Cursors2");
            titleButtons = LoadTexture(content, "Minigames/TitleButtons.pt-BR", "Minigames/TitleButtons");
            clouds = LoadTexture(content, "Minigames/Clouds");
            dayBackground = LoadTexture(content, "LooseSprites/daybg");
            maleBase = LoadTexture(content, "Characters/Farmer/farmer_base");
            femaleBase = LoadTexture(content, "Characters/Farmer/farmer_girl_base");
            hairTexture = LoadTexture(content, "Characters/Farmer/hairstyles");
            hairTexture2 = LoadTexture(content, "Characters/Farmer/hairstyles2");
            shirtTexture = LoadTexture(content, "Characters/Farmer/shirts");
            pantsTexture = LoadTexture(content, "Characters/Farmer/pants");
            accessoryTexture = LoadTexture(content, "Characters/Farmer/accessories");

            skinTexture = LoadTexture(content, "Characters/Farmer/skinColors");
            shoeTexture = LoadTexture(content, "Characters/Farmer/shoeColors");
            // Cada tom de pele ocupa 3 pixels (sombra, base, luz); usamos a base.
            skinColors = ReadPalette(skinTexture, 3, 1, 24, SkinFallback);
            shoeColors = ReadPalette(shoeTexture, 4, 0, 19, ShoeFallback);
        }

        private static Color[] ReadPalette(Texture2D texture, int group, int offset, int count, Color[] fallback)
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
                    var index = i * group + offset;
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
            if (warningMs > 0)
                warningMs -= (int)Math.Min(gameTime.ElapsedGameTime.TotalMilliseconds, 50);

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

            if (Contains(AdvancedRect, point))
                return new AdvancedOptionsScreen(draft);

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

            if (Contains(OkRect, point))
            {
                if (!CanStart)
                {
                    // O original nao deixa sair sem os tres campos: mostra um
                    // aviso e mantem o jogador na criacao.
                    warningMs = 3200;
                    return this;
                }

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

        private void Randomize()
        {
            // Mesmos intervalos do CharacterCustomization.randomButton da 1.5.6.
            // O facing NAO e sorteado: o preview permanece olhando para a
            // camera, apoiado na sombra do daybg.
            draft.IsMale = random.Next(2) == 0;
            draft.Skin = random.NextDouble() < 0.25 ? random.Next(24) : random.Next(6);
            // Masculino: 0..15; feminino: 16..31 (os unicos cabelos seguros
            // do sheet basico, evitando os indices especiais 56..111).
            draft.Hair = draft.IsMale ? random.Next(16) : random.Next(16, 32);
            draft.HairColor = random.Next(HairPalette.Length);
            draft.EyeColor = random.Next(EyePalette.Length);
            draft.Shirt = random.Next(112);
            draft.ShirtColor = random.Next(ShirtPalette.Length);
            draft.Pants = random.Next(20);
            draft.PantsColor = random.Next(PantsPalette.Length);
            draft.Shoes = random.Next(shoeColors.Length);
            draft.Accessory = random.NextDouble() < 0.33
                ? (draft.IsMale ? random.Next(19) : random.Next(6, 19))
                : -1;
            draft.Pet = random.Next(1, 3);
            draft.Facing = 2;
            SyncPickers();
        }

        private void Adjust(int row, int delta)
        {
            switch (row)
            {
                case 0: draft.Skin = Wrap(draft.Skin + delta, skinColors.Length); break;
                case 1: draft.Hair = Wrap(draft.Hair + delta, 32); break;
                case 2: draft.Shirt = Wrap(draft.Shirt + delta, 20); break;
                case 3: draft.Pants = Wrap(draft.Pants + delta, 20); break;
                case 4:
                    if (draft.Accessory < 1 && delta > 0) draft.Accessory = 1;
                    else if (draft.Accessory == 1 && delta < 0) draft.Accessory = 0;
                    else draft.Accessory = Wrap(draft.Accessory + delta, 20);
                    break;
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

        /// <summary>Le os HSV atuais das paletas para posicionar os cursores.</summary>
        private void SyncPickers()
        {
            SetPickerFromColor(0, EyePalette[draft.EyeColor % EyePalette.Length]);
            SetPickerFromColor(1, HairPalette[draft.HairColor % HairPalette.Length]);
            SetPickerFromColor(2, PantsPalette[draft.PantsColor % PantsPalette.Length]);
        }

        private void SetPickerFromColor(int kind, Color color)
        {
            float h, s, v;
            RgbToHsv(color, out h, out s, out v);
            pickerValues[kind * 3] = (int)(h / 360f * 100f);
            pickerValues[kind * 3 + 1] = (int)(s * 100f);
            // RgbToHsv devolve v em 0..1 (o ColorPicker original usa 0..255).
            pickerValues[kind * 3 + 2] = (int)(v * 100f);
        }

        private void ApplyPicker(int kind)
        {
            var color = HsvToRgb(
                pickerValues[kind * 3] / 100f * 360f,
                pickerValues[kind * 3 + 1] / 100f,
                pickerValues[kind * 3 + 2] / 100f);
            var palette = kind == 0 ? EyePalette : kind == 1 ? HairPalette : PantsPalette;
            var index = Nearest(color, palette);
            if (kind == 0) draft.EyeColor = index;
            else if (kind == 1) draft.HairColor = index;
            else draft.PantsColor = index;
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
                    pickerValues[picker * 3 + bar] = (int)(t * 100f);
                    ApplyPicker(picker);
                    return true;
                }
            }

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Fundo proprio: ceu/montanhas/nuvens da home, sem logo nem botoes.
            DrawBackground(spriteBatch);

            DrawMenuBox(spriteBatch, MenuRect);
            DrawBackButton(spriteBatch);
            DrawPortraitBackground(spriteBatch);
            DrawGenderAndPet(spriteBatch);
            DrawAppearance(spriteBatch);
            DrawFarmPanel(spriteBatch);
            DrawAdvancedButton(spriteBatch);
            DrawOk(spriteBatch);
            DrawColorPickers(spriteBatch);
            DrawTextBoxes(spriteBatch);
            DrawSkipIntro(spriteBatch);
            DrawRandomButton(spriteBatch);
            DrawFarmer(spriteBatch);
            DrawFarmHint(spriteBatch);
            DrawWarning(spriteBatch);

            keyboard.Draw(spriteBatch, pixel, font, activeField == 2
                ? "Nome da fazenda"
                : activeField == 3 ? "Coisa favorita" : "Nome do fazendeiro");
        }

        /// <summary>
        /// TitleMenu.draw com subMenu aberto: azul de base, coluna de ceu,
        /// montanhas e nuvens com a camera assentada (viewportY = -1000),
        /// sem logo e sem os botoes da home. Os retangulos sao maiores que o
        /// virtual 1280x720 porque a transformacao da tela de criacao
        /// (UiScale) deixa as bordas do alvo 640x360 descobertas.
        /// </summary>
        private void DrawBackground(SpriteBatch batch)
        {
            batch.Draw(pixel, new Rectangle(-400, -400, 2400, 1600), SkyColor);

            if (cursors != null)
            {
                SafeDraw(batch, cursors, new Rectangle(-400, -240, 1840, 1260),
                    new Rectangle(703, 1912, 1, 264), Color.White);
                SafeDraw(batch, cursors, new Rectangle(-400, -420, 2552, 780),
                    new Rectangle(0, 1453, 638, 195), Color.White * 0.85f);
            }

            if (clouds == null)
                return;

            SafeDraw(batch, clouds, new Rectangle(-100, 470, 1536, 1011),
                new Rectangle(0, 0, 512, 337), Color.White * 0.95f);
            SafeDraw(batch, clouds, new Rectangle(1000, 470, 1536, 1011),
                new Rectangle(0, 0, 512, 337), Color.White * 0.95f);

            Rectangle[] smallClouds =
            {
                new Rectangle(152, 447, 123, 55),
                new Rectangle(0, 471, 149, 66),
                new Rectangle(410, 467, 63, 37)
            };
            for (var i = 0; i < 16; i++)
            {
                var src = smallClouds[i % smallClouds.Length];
                SafeDraw(batch, clouds,
                    new Rectangle(80 + i * 96, 320 - i * 36, src.Width * 3, src.Height * 3),
                    src, Color.White * 0.9f);
            }
        }

        private static void SafeDraw(SpriteBatch batch, Texture2D texture, Rectangle dest,
            Rectangle source, Color color)
        {
            if (texture == null || source.X < 0 || source.Y < 0 ||
                source.Right > texture.Width || source.Bottom > texture.Height)
                return;
            batch.Draw(texture, dest, source, color);
        }

        /// <summary>
        /// Game1.drawDialogueBox: o miolo bege comeca 28px dentro e as bordas
        /// sao desenhadas SEM offset. As bordas tem area transparente que o
        /// miolo preenche, entao nao aparece céu entre a moldura e o bege.
        /// </summary>
        private void DrawMenuBox(SpriteBatch batch, Rectangle area)
        {
            if (menuTiles == null || menuTiles.Width < 256 || menuTiles.Height < 256)
            {
                batch.Draw(pixel, area, new Color(24, 48, 32));
                return;
            }

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

        /// <summary>IClickableMenu.drawTextureBox: canto de 20px e sombra opcional.</summary>
        private void DrawSmallBox(SpriteBatch batch, Rectangle area, bool shadow)
        {
            if (menuTiles == null || menuTiles.Width < 60 || menuTiles.Height < 316)
            {
                batch.Draw(pixel, area, new Color(24, 48, 32));
                return;
            }

            const int b = 20;
            var src = new Rectangle(0, 256, 60, 60);
            if (shadow)
            {
                DrawBox9(batch, menuTiles, new Rectangle(area.X - 8, area.Y + 8, area.Width, area.Height),
                    src, b, Color.Black * 0.4f);
            }
            DrawBox9(batch, menuTiles, area, src, b, Color.White);
        }

        private static void DrawBox9(SpriteBatch batch, Texture2D texture, Rectangle area,
            Rectangle src, int b, Color color)
        {
            batch.Draw(texture, new Rectangle(area.X + b, area.Y + b, area.Width - b * 2, area.Height - b * 2),
                new Rectangle(src.X + b, src.Y + b, b, b), color);
            batch.Draw(texture, new Vector2(area.X, area.Y), new Rectangle(src.X, src.Y, b, b), color,
                0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            batch.Draw(texture, new Vector2(area.Right - b, area.Y), new Rectangle(src.X + b * 2, src.Y, b, b), color,
                0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            batch.Draw(texture, new Vector2(area.X, area.Bottom - b), new Rectangle(src.X, src.Y + b * 2, b, b), color,
                0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            batch.Draw(texture, new Vector2(area.Right - b, area.Bottom - b),
                new Rectangle(src.X + b * 2, src.Y + b * 2, b, b), color,
                0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            batch.Draw(texture, new Rectangle(area.X + b, area.Y, area.Width - b * 2, b),
                new Rectangle(src.X + b, src.Y, b, b), color);
            batch.Draw(texture, new Rectangle(area.X + b, area.Bottom - b, area.Width - b * 2, b),
                new Rectangle(src.X + b, src.Y + b * 2, b, b), color);
            batch.Draw(texture, new Rectangle(area.X, area.Y + b, b, area.Height - b * 2),
                new Rectangle(src.X, src.Y + b, b, b), color);
            batch.Draw(texture, new Rectangle(area.Right - b, area.Y + b, b, area.Height - b * 2),
                new Rectangle(src.X + b * 2, src.Y + b, b, b), color);
        }

        private void DrawPortraitBackground(SpriteBatch batch)
        {
            if (dayBackground != null)
                batch.Draw(dayBackground, new Vector2(PortraitRect.X, PortraitRect.Y), null, Color.White,
                    0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            else
                batch.Draw(pixel, PortraitRect, new Color(42, 64, 92));
        }

        /// <summary>
        /// FarmerRenderer.draw: o CharacterCustomization passa layerDepth=0.8
        /// e scale=1, entao a escala final do sprite e 4. Os offsets de camisa,
        /// cabelo e acessorio sao em pixels de sprite e NAO sao multiplicados.
        /// O position (420,84) faz os pes do sprites cairem sobre a sombra do
        /// daybg, exatamente como no preview da 1.5.6.
        /// </summary>
        private void DrawFarmer(SpriteBatch batch)
        {
            const float scale = 4f;
            var pos = new Vector2(PortraitRect.Center.X - 32, PortraitRect.Bottom - 160);
            var facing = Wrap(draft.Facing, 4);
            var body = GetRecoloredBase();

            int bodyY = facing == 0 ? 32 : facing == 1 ? 16 : 0;
            int shirtOffsetY = facing == 0 ? 24 : facing == 1 ? 8 : facing == 3 ? 16 : 0;
            int hairOffsetY = facing == 0 ? 64 : facing == 1 || facing == 3 ? 32 : 0;
            int accessoryOffsetY = facing == 1 || facing == 3 ? 16 : 0;
            int hairDrawY = facing == 0 ? 4 : 0;
            var flipHair = facing == 3;
            var hairXSign = facing == 3 ? -1f : 1f;

            // Corpo (pele ja recolorida nos indices 260..262 do base).
            if (body != null)
                DrawSpriteScaled(batch, body, pos, new Rectangle(0, bodyY, 16, 32), Color.White, scale);

            // Calca: mesmo calculo do FarmerRenderer, incluindo todos os estilos.
            if (pantsTexture != null)
            {
                var style = Wrap(draft.Pants, 20);
                var src = new Rectangle(
                    style % 10 * 192 + (draft.IsMale ? 0 : 96),
                    style / 10 * 688,
                    16, 32);
                DrawSpriteScaled(batch, pantsTexture, pos, src,
                    PantsPalette[draft.PantsColor % PantsPalette.Length], scale);
            }

            // Olhos: o preview parado nao pisca e olha para a camara.
            if (body != null && facing != 2)
            {
                var eyeX = 5 + (facing == 1 ? 12 : 4);
                DrawSpriteScaled(batch, body,
                    pos + new Vector2(eyeX * 4f, (facing == 0 ? 44 : 40)),
                    new Rectangle(5, 16, 2, 2), Color.White, scale);
                DrawSpriteScaled(batch, body,
                    pos + new Vector2(eyeX * 4f, facing == 1 || facing == 3 ? 40 : 44),
                    new Rectangle(264 + (facing == 3 ? 4 : 0), 2, 2, 2), Color.White, scale);
            }

            // Camisa: ate o indice 111, como o ClampShirt do original.
            if (shirtTexture != null && draft.Shirt >= 0 && draft.Shirt < 112)
            {
                var shirtX = draft.Shirt * 8 % 128;
                var shirtY = draft.Shirt * 8 / 128 * 32 + shirtOffsetY;
                var shirtPos = pos + new Vector2(16f, 56f);
                DrawSpriteScaled(batch, shirtTexture, shirtPos,
                    new Rectangle(shirtX, shirtY, 8, 8), Color.White, scale);
                DrawSpriteScaled(batch, shirtTexture, shirtPos,
                    new Rectangle(shirtX + 128, shirtY, 8, 8),
                    ShirtPalette[draft.ShirtColor % ShirtPalette.Length], scale);
            }

            // Acessorio (sempre desenhado quando >= 0, inclusive de frente).
            if (accessoryTexture != null && draft.Accessory >= 0 && facing != 0)
            {
                var acc = Wrap(draft.Accessory, 20);
                var src = new Rectangle(acc * 16 % 128, acc * 16 / 128 * 32 + accessoryOffsetY, 16, 16);
                var accPos = pos + new Vector2(0f, facing == 2 ? 4f : 4f);
                var accColor = draft.Accessory < 6
                    ? HairPalette[draft.HairColor % HairPalette.Length]
                    : Color.White;
                DrawSpriteScaled(batch, accessoryTexture, accPos, src, accColor, scale,
                    flipHair ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            }

            Texture2D hair;
            Rectangle hairSrc;
            if (draft.Hair < HairstylesPerSheet || hairTexture2 == null)
            {
                // hairstyles.xnb: 8 colunas x 7 fileiras de 16x32.
                hair = hairTexture;
                hairSrc = new Rectangle(draft.Hair % 8 * 16, draft.Hair / 8 * 96 + hairOffsetY, 16, 32);
            }
            else
            {
                // HairData mapeia 100..111 para hairstyles2.xnb: duas fileiras.
                var index = Wrap(draft.Hair - 100, 12);
                hair = hairTexture2;
                hairSrc = new Rectangle(index % 8 * 16, index / 8 * 128 + hairOffsetY, 16, 32);
            }

            if (hair != null)
            {
                var hairPos = pos + new Vector2(0f, hairDrawY);
                var hairXOffset = draft.IsMale && draft.Hair >= 16 ? -4f
                    : !draft.IsMale && draft.Hair < 16 ? 4f : 0f;
                hairPos.X += hairXSign * hairXOffset;
                DrawSpriteScaled(batch, hair, hairPos, hairSrc,
                    HairPalette[draft.HairColor % HairPalette.Length], scale,
                    flipHair ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            }

            // Braco direito do sheet base, por cima do cabelo.
            if (body != null)
                DrawSpriteScaled(batch, body, pos, new Rectangle(96, bodyY, 16, 32), Color.White, scale);
        }

        private Texture2D GetRecoloredBase()
        {
            var male = draft.IsMale;
            var source = male ? maleBase : femaleBase;
            if (source == null)
                return null;
            if (recoloredBase != null && recoloredSkin == draft.Skin && recoloredMale == male)
                return recoloredBase;

            var data = new Color[source.Width * source.Height];
            source.GetData(data);

            // Pele: trios sombra/base/luz do skinColors, como o ApplySkinColor.
            if (skinTexture != null && skinTexture.Width == 3)
            {
                var palette = new Color[skinTexture.Width * skinTexture.Height];
                skinTexture.GetData(palette);
                var which = Wrap(draft.Skin, skinTexture.Height);
                _SwapColor(data, 260, palette[which * 3]);
                _SwapColor(data, 261, palette[which * 3 + 1]);
                _SwapColor(data, 262, palette[which * 3 + 2]);
            }

            // Sapato: o ApplyShoeColor troca os indices 268..271 do base,
            // usando quatro cores por linha do shoeColors. Sem isso os pes
            // aparecem com a cor errada (ou parecem faltar).
            if (shoeTexture != null && shoeTexture.Width == 4)
            {
                var palette = new Color[shoeTexture.Width * shoeTexture.Height];
                shoeTexture.GetData(palette);
                var which = Wrap(draft.Shoes, shoeTexture.Height);
                for (var i = 0; i < 4; i++)
                    _SwapColor(data, 268 + i, palette[which * 4 + i]);
            }

            recoloredBase = new Texture2D(source.GraphicsDevice, source.Width, source.Height);
            recoloredBase.SetData(data);
            recoloredSkin = draft.Skin;
            recoloredMale = male;
            return recoloredBase;
        }

        private static void _SwapColor(Color[] pixels, int index, Color color)
        {
            if (index < 0 || index >= pixels.Length)
                return;
            for (var i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].A == 0 || pixels[i].R != index || pixels[i].G != index || pixels[i].B != index)
                    continue;
                pixels[i] = color;
            }
        }

        private static void DrawSpriteScaled(SpriteBatch batch, Texture2D texture, Vector2 position,
            Rectangle source, Color color, float scale, SpriteEffects effects = SpriteEffects.None)
        {
            if (texture == null)
                return;
            if (source.X < 0 || source.Y < 0 ||
                source.Right > texture.Width || source.Bottom > texture.Height)
                return;
            batch.Draw(texture, position, source, color, 0f, Vector2.Zero, scale, effects, 0f);
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

            ShadowText(batch, Localization.UIString("Strings\\UI:Character_Animal", "Animal\nfavorito"),
                540, 252, TextColor, 1.1f);
        }

        private void DrawAppearance(SpriteBatch batch)
        {
            for (var row = 0; row < 5; row++)
            {
                DrawArrow(batch, AppearanceArrowRect(row, true), true);
                DrawArrow(batch, AppearanceArrowRect(row, false), false);
            }

            var labelX = 418;
            for (var row = 0; row < 5; row++)
            {
                var y = 340 + row * 68;
                var label = AppearanceLabel(row);
                DrawCenteredLabel(batch, label, labelX, y);

                var value = row == 4 && draft.Accessory < 0
                    ? Localization.UIString("Strings\\UI:Character_none", "Nenhum")
                    : GetAppearanceValue(row).ToString();
                DrawCenteredValue(batch, value, labelX, y + 32);
            }

            ShadowText(batch, Localization.UIString("Strings\\UI:Character_EyeColor", "Cor dos\nolhos"),
                556, 340, TextColor, 1.1f);
            ShadowText(batch, Localization.UIString("Strings\\UI:Character_HairColor", "Cor de\ncabelo"),
                556, 408, TextColor, 1.1f);
            ShadowText(batch, Localization.UIString("Strings\\UI:Character_PantsColor", "Cor da\ncalça"),
                556, 476, TextColor, 1.1f);
        }

        private int GetAppearanceValue(int row)
        {
            switch (row)
            {
                case 0: return draft.Skin + 1;
                case 1: return draft.Hair + 1;
                case 2: return draft.Shirt + 1;
                case 3: return draft.Pants + 1;
                default: return draft.Accessory + 2;
            }
        }

        private void DrawCenteredLabel(SpriteBatch batch, string text, int x, int y)
        {
            if (font == null || string.IsNullOrEmpty(text))
                return;
            var offset = 21f - font.MeasureString(text).X / 2f;
            ShadowText(batch, text, x + offset, y, TextColor, 1.1f);
        }

        private void DrawCenteredValue(SpriteBatch batch, string text, int x, int y)
        {
            if (font == null || string.IsNullOrEmpty(text))
                return;
            var px = x + 21f - font.MeasureString(text).X / 2f;
            ShadowText(batch, text, px, y, TextColor, 1.1f);
        }

        private static string AppearanceLabel(int row)
        {
            switch (row)
            {
                case 0: return Localization.UIString("Strings\\UI:Character_Skin", "Pele");
                case 1: return Localization.UIString("Strings\\UI:Character_Hair", "Cabelo");
                case 2: return Localization.UIString("Strings\\UI:Character_Shirt", "Camisa");
                case 3: return Localization.UIString("Strings\\UI:Character_Pants", "Calças");
                default: return Localization.UIString("Strings\\UI:Character_Accessory", "Acess.");
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
            DrawColorPicker(batch, 0, 324);
            DrawColorPicker(batch, 1, 392);
            DrawColorPicker(batch, 2, 460);
        }

        private void DrawColorPicker(SpriteBatch batch, int kind, int y)
        {
            const int x = 708;
            var hue = pickerValues[kind * 3];
            var saturation = pickerValues[kind * 3 + 1];
            var value = pickerValues[kind * 3 + 2];

            // 24 fatias de 5px (+1 para nao abrir costuras apos a escala).
            const int chunk = 128 / 24 + 1;
            for (var i = 0; i < 24; i++)
                batch.Draw(pixel, new Rectangle(x + i * 5, y + 8, chunk, 4),
                    HsvToRgb(i / 24f * 360f, 0.9f, 0.9f));
            DrawPickerCursor(batch, x + (int)(hue / 100f * 128f), y + 10);

            for (var i = 0; i < 24; i++)
                batch.Draw(pixel, new Rectangle(x + i * 5, y + 28, chunk, 4),
                    HsvToRgb(hue / 100f * 360f, i / 24f, value / 100f));
            DrawPickerCursor(batch, x + (int)(saturation / 100f * 128f), y + 30);

            for (var i = 0; i < 24; i++)
                batch.Draw(pixel, new Rectangle(x + i * 5, y + 48, chunk, 4),
                    HsvToRgb(hue / 100f * 360f, saturation / 100f, i / 24f));
            DrawPickerCursor(batch, x + (int)(value / 100f * 128f), y + 50);

            ShadowText(batch, hue.ToString(), x + 136, y, TextColor, 1.1f);
            ShadowText(batch, saturation.ToString(), x + 136, y + 20, TextColor, 1.1f);
            ShadowText(batch, value.ToString(), x + 136, y + 40, TextColor, 1.1f);
        }

        private void DrawPickerCursor(SpriteBatch batch, float x, float y)
        {
            if (cursors == null)
            {
                batch.Draw(pixel, new Rectangle((int)x, (int)y, 4, 4), Color.White);
                return;
            }
            batch.Draw(cursors, new Vector2(x, y), new Rectangle(64, 256, 32, 32), Color.White,
                0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0f);
        }

        private void DrawTextBoxes(SpriteBatch batch)
        {
            DrawTextBox(batch, NameRect, draft.FarmerName,
                "Strings\\UI:Character_Name", "Nome", 548, 60, 1);
            DrawTextBox(batch, FarmNameRect, draft.FarmName,
                "Strings\\UI:Character_Farm", "Fazenda", 540, 116, 2);
            DrawTextBox(batch, FavoriteRect, draft.FavoriteThing,
                "Strings\\UI:Character_FavoriteThing", "Coisa\nfavorita", 548, 180, 3);
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
                batch.DrawString(font, value, new Vector2(rect.X + 16, rect.Y + 12), TextColor);

            var empty = string.IsNullOrWhiteSpace(value);
            var labelColor = empty ? Color.Red : TextColor;
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

            ShadowText(batch, Localization.UIString("Strings\\UI:Character_SkipIntro", "Pular introducao"),
                SkipIntroRect.Right + 8, SkipIntroRect.Y + 8, TextColor, 1.1f);
        }

        private void DrawRandomButton(SpriteBatch batch)
        {
            if (cursors != null)
                batch.Draw(cursors, RandomRect, new Rectangle(381, 361, 10, 10), Color.White);
        }

        private void DrawFarmPanel(SpriteBatch batch)
        {
            DrawSmallBox(batch, FarmPanelRect, true);

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
        }

        /// <summary>
        /// IClickableMenu.drawTextureBox com source (375,357,3,3) e scale 4:
        /// num = 1, entao cada lado da moldura tem 4px. O centro do 9-slice
        /// nao e desenhado (era ele que produzia o quadrado escuro).
        /// </summary>
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

            const int o = 4;
            var s = new Rectangle(375, 357, 1, 1);
            // Centro (apenas uma linha de 1px escalada, sem preenchimento).
            batch.Draw(cursors, new Rectangle(area.X + o, area.Y + o, area.Width - o * 2, area.Height - o * 2),
                new Rectangle(376, 358, 1, 1), Color.White);
            batch.Draw(cursors, new Vector2(area.X, area.Y), s, Color.White);
            batch.Draw(cursors, new Vector2(area.Right - o, area.Y), new Rectangle(377, 357, 1, 1), Color.White);
            batch.Draw(cursors, new Vector2(area.Right - o, area.Bottom - o), new Rectangle(377, 359, 1, 1), Color.White);
            batch.Draw(cursors, new Vector2(area.X, area.Bottom - o), new Rectangle(375, 359, 1, 1), Color.White);
            batch.Draw(cursors, new Rectangle(area.X + o, area.Y, area.Width - o * 2, o), new Rectangle(376, 357, 1, 1), Color.White);
            batch.Draw(cursors, new Rectangle(area.X + o, area.Bottom - o, area.Width - o * 2, o), new Rectangle(376, 359, 1, 1), Color.White);
            batch.Draw(cursors, new Rectangle(area.X, area.Y + o, o, area.Height - o * 2), new Rectangle(375, 358, 1, 1), Color.White);
            batch.Draw(cursors, new Rectangle(area.Right - o, area.Y + o, o, area.Height - o * 2), new Rectangle(377, 358, 1, 1), Color.White);
        }

        private void DrawOk(SpriteBatch batch)
        {
            if (cursors != null)
            {
                // getSourceRectForStandardTileSheet(mouseCursors, 46) com o
                // tile padrao de 64px: (46*64 % 704, 46*64 / 704 * 64, 64, 64).
                batch.Draw(cursors, OkRect, new Rectangle(128, 256, 64, 64), Color.White);
            }
            else
                batch.Draw(pixel, OkRect, new Color(96, 142, 72));
        }

        private void DrawAdvancedButton(SpriteBatch batch)
        {
            if (cursors2 != null)
                batch.Draw(cursors2, AdvancedRect, new Rectangle(154, 154, 20, 20), Color.White);
            else
                batch.Draw(pixel, AdvancedRect, new Color(48, 82, 54));
        }

        private void DrawBackButton(SpriteBatch batch)
        {
            if (titleButtons != null)
                batch.Draw(titleButtons, BackButtonRect, new Rectangle(296, 252, 66, 27), Color.White);
            else
                DrawRow(batch, BackButtonRect, Localization.CS("StringsFromCSFiles:TitleMenu.cs.11739", "Voltar"), false);
        }

        /// <summary>Aviso exibido quando o jogador tenta confirmar sem preencher
        /// nome, nome da fazenda e coisa favorita.</summary>
        private void DrawWarning(SpriteBatch batch)
        {
            if (warningMs <= 0)
                return;

            var area = new Rectangle(360, 590, 560, 84);
            batch.Draw(pixel, new Rectangle(area.X - 6, area.Y + 6, area.Width + 12, area.Height + 12),
                Color.Black * 0.55f);
            batch.Draw(pixel, area, new Color(28, 58, 38));
            batch.Draw(pixel, new Rectangle(area.X, area.Y, area.Width, 4), new Color(126, 170, 92));
            batch.Draw(pixel, new Rectangle(area.X, area.Bottom - 4, area.Width, 4), new Color(126, 170, 92));

            ShadowText(batch, "Preencha os tres campos antes de comecar:",
                area.X + 14, area.Y + 12, new Color(255, 236, 160), 1f);
            ShadowText(batch, "nome, fazenda e coisa favorita.",
                area.X + 14, area.Y + 44, TextColor, 1f);
        }

        private void DrawFarmHint(SpriteBatch batch)
        {
            if (farmHintMs <= 0)
                return;

            var area = new Rectangle(300, 636, 540, 72);
            batch.Draw(pixel, area, Color.Black * 0.82f);
            batch.Draw(pixel, new Rectangle(area.X, area.Y, area.Width, 3), new Color(126, 170, 92));

            string name, description;
            FarmTypes.Parts(draft.FarmType, out name, out description);
            ShadowText(batch, name, area.X + 10, area.Y + 8, new Color(255, 236, 160), 1f);
            DrawWrapped(batch, description, area.X + 10, area.Y + 30, area.Width - 20, area.Height - 34);
        }

        /// <summary>
        /// Equivale a Utility.drawTextWithShadow com smallFont: duas copias da
        /// sombra laranja em (-2,+2) e (0,+2) e o texto na cor padrao.
        /// </summary>
        private void ShadowText(SpriteBatch batch, string text, float x, float y, Color color, float scale)
        {
            if (font == null || string.IsNullOrEmpty(text))
                return;

            var lines = text.Split('\n');
            var lineHeight = font.MeasureString("Ag").Y + 2f;
            for (var i = 0; i < lines.Length; i++)
            {
                var lineY = y + i * lineHeight;
                batch.DrawString(font, lines[i], new Vector2(x - 2f, lineY + 2f),
                    TextShadowColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                batch.DrawString(font, lines[i], new Vector2(x, lineY + 2f),
                    TextShadowColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                batch.DrawString(font, lines[i], new Vector2(x, lineY), color,
                    0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
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
                    ShadowText(batch, line, x, y + lines * lineHeight, TextColor, 1.1f);
                    lines++;
                    line = words[i];
                }
                else
                {
                    line = candidate;
                }
            }

            if (lines < maxLines && line.Length > 0)
                ShadowText(batch, line, x, y + lines * lineHeight, TextColor, 1.1f);
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
