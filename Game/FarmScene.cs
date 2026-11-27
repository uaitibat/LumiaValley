using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LumiaValley.Game.Map;

namespace LumiaValley.Game
{
    /// <summary>
    /// Cena jogavel da fazenda: mapa xTile real da 1.5.6, camera seguindo o
    /// jogador, D-pad virtual e HUD basico. Tudo em coordenadas virtuais.
    /// </summary>
    public sealed class FarmScene
    {
        private Texture2D pixel;
        private Texture2D cursors;
        private Texture2D shadow;
        private Texture2D windowLight;
        private Texture2D farmerBase;
        private Texture2D hairSheet;
        private Texture2D shirtSheet;
        private Texture2D pantSheet;
        private Texture2D accessorySheet;
        private Texture2D skinSheet;
        private SpriteFont font;
        private TileMap map;
        private readonly Dictionary<string, Texture2D> sheets = new Dictionary<string, Texture2D>();
        private readonly FurnitureRenderer furniture = new FurnitureRenderer();

        private Vector2 player;
        private Vector2 target;
        private bool hasTarget;
        private Vector2 direction;
        private Vector2 camera;

        private readonly SaveSlot slot;
        private double elapsedMs;
        private int gameMinutes = 360;
        private int energy = 270;
        private int facing = 2;
        private int walkFrame;
        private double walkMs;

        public FarmScene(SaveSlot slot = null)
        {
            this.slot = slot;
        }

        public void Load(ContentManager content, GraphicsDevice device)
        {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] { Color.White });

            try { font = content.Load<SpriteFont>("Fonts/SmallFont.pt-BR"); }
            catch { font = null; }
            try { cursors = content.Load<Texture2D>("LooseSprites/Cursors.pt-BR"); }
            catch { cursors = null; }
            try { shadow = content.Load<Texture2D>("LooseSprites/shadow"); }
            catch { shadow = null; }
            try { windowLight = content.Load<Texture2D>("LooseSprites/Lighting/indoorWindowLight"); }
            catch { windowLight = null; }
            try { furniture.Load(content); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Furniture: " + ex.Message); }

            try
            {
                farmerBase = content.Load<Texture2D>("Characters/Farmer/farmer_base");
                hairSheet = content.Load<Texture2D>("Characters/Farmer/hairstyles");
                shirtSheet = content.Load<Texture2D>("Characters/Farmer/shirts");
                pantSheet = content.Load<Texture2D>("Characters/Farmer/pants");
                accessorySheet = content.Load<Texture2D>("Characters/Farmer/accessories");
                skinSheet = content.Load<Texture2D>("Characters/Farmer/skinColors");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Sprites do fazendeiro: " + ex.Message);
            }

            try { map = content.Load<TileMap>("Maps/FarmHouse"); }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao carregar Maps/FarmHouse: " + ex.Message);
                map = null;
            }

            if (map != null)
                ApplyDefaultDecorations();

            if (map != null)
            {
                foreach (var sheet in map.TileSheets)
                {
                    if (string.IsNullOrEmpty(sheet.ImageSource))
                        continue;
                    var path = "Maps/" + sheet.ImageSource;
                    try
                    {
                        sheets[sheet.Id] = content.Load<Texture2D>(path + ".pt-BR");
                    }
                    catch
                    {
                        try { sheets[sheet.Id] = content.Load<Texture2D>(path); }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Tilesheet " + path + ": " + ex.Message);
                        }
                    }
                }
            }

            Reset();
        }

        /// <summary>
        /// Porte do DecoratableLocation.ReadWallpaperAndFloorTileData +
        /// UpdateWallpaper/UpdateFloor com o papel/piso padrao (0), que o
        /// original aplica ao entrar na casa pela primeira vez.
        /// </summary>
        private void ApplyDefaultDecorations()
        {
            var back = map.Layers.Count > 0 ? map.Layers[0] : null;
            var buildings = map.Layers.Count > 1 ? map.Layers[1] : null;
            if (back == null)
                return;

            const int sheetWidth = 16; // walls_and_floors

            for (var y = 0; y < back.Height; y++)
            {
                for (var x = 0; x < back.Width; x++)
                {
                    var tile = back.Get(x, y);
                    if (tile == null || !tile.Properties.ContainsKey("WallID"))
                        continue;

                    // Padrao 0: value = indice na tilesheet; o original faz
                    // value/sheetWidth*sheetWidth*3 + value%sheetWidth + linha.
                    var value = 0;
                    for (var row = 0; row < 3; row++)
                    {
                        var layer = row == 2 && buildings != null ? buildings : back;
                        var ty = y + row;
                        var target = layer.Get(x, ty);
                        if (target == null || target.Sheet == null ||
                            target.Sheet.Id != "walls_and_floors")
                            continue;
                        target.Index = value / sheetWidth * sheetWidth * 3 + value % sheetWidth + row * sheetWidth;
                    }
                }
            }

            // O original nao troca o piso da casa inicial (whichFarm=0 nao
            // chama setFloor), entao mantemos os indices do mapa.
        }

        public void Reset()
        {
            // Save novo: o jogador nasce deitado na cama (9,8), parado.
            player = new Vector2(9 * 64f, 9 * 64f);
            target = player;
            hasTarget = false;
            facing = 2;
            direction = Vector2.Zero;
            UpdateCamera();
        }

        /// <summary>Transformacao aplicada pelo FarmScreen antes do desenho.</summary>
        public Matrix Transform
        {
            get { return Matrix.CreateTranslation(-camera.X, -camera.Y, 0f); }
        }

        public void OnTap(Vector2 screenPoint)
        {
            // Sem D-pad: apenas toque no mundo, como no original em desktop.
            target = screenPoint + camera;
            hasTarget = true;
        }

        public void ReleaseDirection()
        {
            direction = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)Math.Min(gameTime.ElapsedGameTime.TotalSeconds, 0.05);
            elapsedMs += dt * 1000;
            gameMinutes += (int)(dt * 12); // um dia de 20 minutos

            const float speed = 192f;
            if (hasTarget)
            {
                var delta = target - player;
                var distance = delta.Length();
                var step = speed * dt;
                if (distance <= step)
                {
                    player = target;
                    hasTarget = false;
                }
                else
                {
                    player += delta / distance * step;
                    // Farmer.updateMovementAnimation: direcao pelo movimento.
                    facing = Math.Abs(delta.X) > Math.Abs(delta.Y)
                        ? (delta.X > 0 ? 1 : 3)
                        : (delta.Y > 0 ? 2 : 0);

                    // FarmerSprite.getAnimationFromIndex: 4 frames de 200ms.
                    walkMs += dt * 1000;
                    if (walkMs >= 200)
                    {
                        walkMs -= 200;
                        walkFrame = (walkFrame + 1) % 4;
                    }
                }
            }
            else
            {
                walkFrame = 0;
                walkMs = 0;
            }

            ClampPlayer();
            UpdateCamera();
        }

        private void ClampPlayer()
        {
            if (map == null)
                return;
            var maxX = map.Width * 64f;
            var maxY = map.Height * 64f;
            player.X = MathHelper.Clamp(player.X, 32f, maxX - 32f);
            player.Y = MathHelper.Clamp(player.Y, 48f, maxY - 16f);
        }

        private void UpdateCamera()
        {
            var worldWidth = map != null ? map.Width * 64 : ViewportConfig.VirtualWidth;
            var worldHeight = map != null ? map.Height * 64 : ViewportConfig.VirtualHeight;

            // Mapas menores que a viewport ficam centralizados, como no original.
            camera.X = worldWidth <= ViewportConfig.VirtualWidth
                ? -(ViewportConfig.VirtualWidth - worldWidth) / 2f
                : MathHelper.Clamp(player.X - ViewportConfig.VirtualWidth / 2f,
                    0f, worldWidth - ViewportConfig.VirtualWidth);
            camera.Y = worldHeight <= ViewportConfig.VirtualHeight
                ? -(ViewportConfig.VirtualHeight - worldHeight) / 2f
                : MathHelper.Clamp(player.Y - ViewportConfig.VirtualHeight / 2f,
                    0f, worldHeight - ViewportConfig.VirtualHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Ordem do Game1.draw: fundo -> objetos/jogador -> Front/AlwaysFront.
            DrawMap(spriteBatch, false);
            DrawLights(spriteBatch);
            DrawFurniture(spriteBatch);
            DrawFarmer(spriteBatch);
            DrawMap(spriteBatch, true);
            DrawHud(spriteBatch);
        }

        private void DrawMap(SpriteBatch batch, bool front)
        {
            if (map == null)
            {
                if (!front)
                    batch.Draw(pixel, new Rectangle(0, 0, ViewportConfig.VirtualWidth,
                        ViewportConfig.VirtualHeight), new Color(74, 132, 62));
                return;
            }

            foreach (var layer in map.Layers)
            {
                if (layer == null || !layer.Visible || layer.Tiles == null)
                    continue;

                // Back/Buildings antes dos objetos; Front/AlwaysFront depois.
                var isFront = layer.Id == "Front" || layer.Id == "AlwaysFront";
                if (isFront != front)
                    continue;

                for (var i = 0; i < layer.Tiles.Length; i++)
                {
                    var tile = layer.Tiles[i];
                    if (tile == null || tile.Sheet == null)
                        continue;
                    Texture2D texture;
                    if (!sheets.TryGetValue(tile.Sheet.Id, out texture) || texture == null)
                        continue;

                    var col = tile.Index % tile.Sheet.SheetWidth;
                    var row = tile.Index / tile.Sheet.SheetWidth;
                    var src = new Rectangle(
                        col * tile.Sheet.TileWidth + tile.Sheet.MarginX,
                        row * tile.Sheet.TileHeight + tile.Sheet.MarginY,
                        tile.Sheet.TileWidth, tile.Sheet.TileHeight);
                    if (src.X < 0 || src.Y < 0 || src.Right > texture.Width || src.Bottom > texture.Height)
                        continue;

                    var x = i % layer.Width * 64;
                    var y = i / layer.Width * 64;
                    batch.Draw(texture, new Rectangle(x, y, 64, 64), src, Color.White);
                }
            }
        }

        /// <summary>Luz da janela: Game1.drawLighting com indoorWindowLight.</summary>
        private void DrawLights(SpriteBatch batch)
        {
            if (windowLight == null)
                return;

            // Game1.drawLighting usa BlendState.Additive para as luzes.
            batch.End();
            batch.Begin(blendState: BlendState.Additive,
                samplerState: SamplerState.PointClamp,
                transformMatrix: Game1.SceneTransform);

            // Janela em (3,1): centro do tile, escala do radius do LightSource.
            var center = new Vector2(3 * 64f + 32f, 1 * 64f + 32f);
            batch.Draw(windowLight, center, windowLight.Bounds,
                new Color(255, 244, 190) * 0.28f, 0f,
                new Vector2(windowLight.Width / 2f, windowLight.Height / 2f),
                1.1f, SpriteEffects.None, 0f);

            batch.End();
            batch.Begin(samplerState: SamplerState.PointClamp,
                transformMatrix: Game1.SceneTransform);
        }

        /// <summary>
        /// Moveis que no xTile vem de tiles com a sheet errada no nosso
        /// parser. Usamos os indices corretos do townInterior (cama) para
        /// sobrepor as brasoes do walls_and_floors.
        /// </summary>
        private void DrawFurniture(SpriteBatch batch)
        {
            // FarmHouse.doAction / Game1.loadForNewGame: moveis iniciais do
            // casal (whichFarm = 0), exatamente como no original.
            // Lista exata de Game1.loadForNewGame / FarmHouse (whichFarm=0).
            furniture.Draw(batch, 2048, 9, 8);   // cama
            furniture.Draw(batch, 1120, 5, 4);   // mesa de carvalho (+ tigela via heldObject)
            furniture.Draw(batch, 1376, 1, 10);  // planta
            furniture.Draw(batch, 0, 4, 4);      // cadeira
            furniture.Draw(batch, 1466, 1, 4);   // TV
            furniture.Draw(batch, 1614, 3, 1);   // janela
            furniture.Draw(batch, 1618, 6, 8);   // tapete
            furniture.Draw(batch, 1602, 5, 1);   // quadro
            furniture.Draw(batch, 1792, 8, 4);   // lareira (8,4 no original)
            furniture.DrawGiftbox(batch, 3, 7);  // bau-presente com 15 parafusos
        }

        /// <summary>Mesmas camadas do preview da criacao: corpo, calca, camisa,
        /// cabelo, braco e acessorio. Escala 4 = 64x128 no espaco do mundo.
        /// </summary>
        private void DrawFarmer(SpriteBatch batch)
        {
            if (farmerBase == null)
                return;

            var pos = new Vector2((float)Math.Floor(player.X - 32), (float)Math.Floor(player.Y - 112));

            // Game1.draw: shadowTexture em player.Position + (32,24),
            // origem no centro, escala 4.
            if (shadow != null)
                batch.Draw(shadow, new Vector2(player.X + 32f, player.Y + 24f),
                    shadow.Bounds, Color.White, 0f,
                    new Vector2(shadow.Bounds.Center.X, shadow.Bounds.Center.Y),
                    4f, SpriteEffects.None, player.Y / 10000f - 0.000001f);

            var male = slot == null || slot.IsMale;
            // Tom fixo da 1.5.6; a leitura do XNB de skinColors no UWP ARM
            // devolve valores inconsistentes, entao usamos o padrao.
            var skin = new Color(224, 172, 140);
            // Paletas identicas as da criacao de personagem.
            var hairPalette = new[]
            {
                new Color(110, 58, 30), new Color(129, 66, 36), new Color(155, 79, 43),
                new Color(193, 90, 50), new Color(224, 137, 83), new Color(246, 191, 121),
                new Color(255, 222, 170), new Color(148, 118, 92), new Color(94, 73, 54),
                new Color(75, 49, 32), new Color(45, 35, 25), new Color(241, 174, 73)
            };
            var shirtPalette = new[]
            {
                new Color(211, 215, 211), new Color(88, 110, 170), new Color(64, 86, 150),
                new Color(50, 62, 120), new Color(148, 76, 70), new Color(118, 60, 56),
                new Color(80, 44, 40), new Color(192, 148, 92), new Color(148, 106, 64),
                new Color(96, 72, 48), new Color(212, 176, 92), new Color(164, 128, 64)
            };
            // Valores do slot: 0 e 1 sao os padroes da 1.5.6 (que o original
            // usa quando o save nao traz cor/estilo explicito).
            var hairIndex = slot == null ? 0 : slot.Hair;
            var hairTone = slot == null ? 0 : slot.HairColor;
            var shirtTone = slot == null ? 1 : slot.ShirtColor;
            var hairColor = hairPalette[Wrap(hairTone, hairPalette.Length)];
            // ShirtColor 0 = branco da 1.5.6, mas ele fica invisivel sobre o
            // fundo claro. Usamos o azul padrao quando o save nao traz cor.
            var shirtColor = shirtTone <= 0
                ? new Color(88, 110, 170)
                : shirtPalette[Wrap(shirtTone, shirtPalette.Length)];
            var pantsPalette = new[]
            {
                new Color(64, 86, 150), new Color(46, 85, 183), new Color(38, 64, 128),
                new Color(88, 110, 170), new Color(148, 76, 70), new Color(118, 60, 56),
                new Color(80, 44, 40), new Color(96, 72, 48), new Color(112, 148, 96),
                new Color(72, 112, 80), new Color(48, 80, 60), new Color(64, 64, 64),
                new Color(126, 92, 140), new Color(180, 180, 180), new Color(192, 148, 92),
                new Color(164, 128, 64), new Color(92, 64, 108), new Color(64, 44, 76),
                new Color(50, 62, 120), new Color(211, 215, 211)
            };
            var eyePalette = new[]
            {
                new Color(96, 64, 40), new Color(128, 88, 56), new Color(160, 116, 76),
                new Color(80, 112, 128), new Color(96, 140, 160), new Color(128, 176, 188),
                new Color(64, 96, 72), new Color(96, 132, 84), new Color(128, 164, 104),
                new Color(112, 72, 120), new Color(148, 104, 148), new Color(180, 148, 180)
            };
            var pantsColor = pantsPalette[Wrap(slot == null ? 0 : slot.PantsColor, pantsPalette.Length)];
            var eyeColor = eyePalette[Wrap(slot == null ? 0 : slot.EyeColor, eyePalette.Length)];

            // FarmerSprite.UpdateSourceRect: frame*16 % 96, frame*16/96*32.
            // Frames idle: cima=12, direita=6, baixo=0, esquerda=6 + flip.
            var flip = facing == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // FarmerSprite.getAnimationFromIndex (linhas 1212-1239).
            var idleFrames = new[] { 12, 6, 0, 6 };
            var walkFrames = new[]
            {
                new[] { 13, 12, 14, 12 }, // cima
                new[] { 7, 6, 8, 6 },     // direita
                new[] { 1, 0, 2, 0 },     // baixo
                new[] { 7, 6, 8, 6 }      // esquerda
            };
            var bodyFrame = hasTarget && walkFrame > 0
                ? walkFrames[Wrap(facing, 4)][walkFrame]
                : idleFrames[Wrap(facing, 4)];
            var bodySrc = new Rectangle(bodyFrame % 6 * 16, bodyFrame / 6 * 32, 16, 32);
            DrawLayer(batch, farmerBase, pos, bodySrc, skin, flip);

            // Olhos (FarmerRenderer:764-781): branco em (5,16), irina em (264,2).
            if (facing != 0)
            {
                var eyeX = facing == 1 ? 8 : facing == 3 ? 6 : 5;
                DrawLayer(batch, farmerBase,
                    pos + new Vector2(eyeX * 4f, facing == 2 ? 44f : 40f),
                    new Rectangle(5, 16, facing == 2 ? 6 : 2, 2), skin, flip);
                DrawLayer(batch, farmerBase,
                    pos + new Vector2(eyeX * 4f, facing == 2 ? 44f : 40f),
                    new Rectangle(264 + (facing == 3 ? 4 : 0), 2, facing == 2 ? 6 : 2, 2),
                    eyeColor, flip);
            }

            if (pantSheet != null && slot != null)
            {
                var style = slot.Pants % 20;
                DrawLayer(batch, pantSheet, pos,
                    new Rectangle(style % 10 * 192 + (male ? 0 : 96), style / 10 * 688, 16, 32),
                    pantsColor, flip);
            }

            if (shirtSheet != null && slot != null && slot.Shirt >= 0 && slot.Shirt < 112)
            {
                var sx = slot.Shirt * 8 % 128;
                var sy = slot.Shirt * 8 / 128 * 32;
                var sp = pos + new Vector2(16, 56);
                DrawLayer(batch, shirtSheet, sp, new Rectangle(sx, sy, 8, 8), Color.White, flip);
                DrawLayer(batch, shirtSheet, sp, new Rectangle(sx + 128, sy, 8, 8), shirtColor, flip);
            }

            if (accessorySheet != null && slot != null && slot.Accessory >= 0)
            {
                var acc = slot.Accessory % 20;
                DrawLayer(batch, accessorySheet, pos,
                    new Rectangle(acc * 16 % 128, acc * 16 / 128 * 32, 16, 16), Color.White, flip);
            }

            if (hairSheet != null)
            {
                var hair = Wrap(hairIndex, 56);
                var hairOffsetY = facing == 0 ? 64 : facing == 1 || facing == 3 ? 32 : 0;
                DrawLayer(batch, hairSheet, pos + new Vector2(0, hairOffsetY),
                    new Rectangle(hair % 8 * 16, hair / 8 * 96, 16, 32), hairColor, flip);
            }

            // Braco direito por cima do cabelo (FarmerRenderer:789).
            DrawLayer(batch, farmerBase, pos,
                new Rectangle(96 + bodyFrame % 6 * 16, bodyFrame / 6 * 32, 16, 32),
                skin, flip);
        }

        private static int Wrap(int value, int count)
        {
            if (count <= 0)
                return 0;
            value %= count;
            return value < 0 ? value + count : value;
        }

        private static void DrawLayer(SpriteBatch batch, Texture2D texture, Vector2 pos,
            Rectangle source, Color color, SpriteEffects effects = SpriteEffects.None)
        {
            if (texture == null || source.X < 0 || source.Y < 0 ||
                source.Right > texture.Width || source.Bottom > texture.Height)
                return;
            batch.Draw(texture, pos, source, color, 0f, Vector2.Zero, 4f, effects, 0f);
        }

        private void DrawHud(SpriteBatch batch)
        {
            // HUD usa coordenadas de tela: somamos a camera para compensar
            // a transformacao do mundo aplicada pelo FarmScreen.
            var ox = (int)camera.X;
            var oy = (int)camera.Y;

            // Porte literal de Game1.drawHUD (linhas 18155-18175 da 1.5.6).
            const float staminaScale = 0.625f;
            const int maxStamina = 270;
            var bx = ViewportConfig.VirtualWidth - 48 - 8 + ox;
            var by = ViewportConfig.VirtualHeight - 224 - 16
                - (int)((maxStamina - 270) * staminaScale) + oy;

            if (cursors != null)
            {
                // Topo, meio e base da moldura (o meio ajusta a altura da viewport).
                batch.Draw(cursors, new Vector2(bx, by), new Rectangle(256, 408, 12, 16),
                    Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                var middleHeight = ViewportConfig.VirtualHeight - 64 - 16 - (int)(by - oy + 64 - 8);
                batch.Draw(cursors, new Rectangle((int)bx, (int)by + 64, 48, middleHeight),
                    new Rectangle(256, 424, 12, 16), Color.White);
                batch.Draw(cursors, new Vector2(bx, by + 224 - 64),
                    new Rectangle(256, 448, 12, 16), Color.White, 0f, Vector2.Zero, 4f,
                    SpriteEffects.None, 1f);

                // Preenchimento: Y = by + 16 + 32 + (maxStamina - stamina)*0.625.
                var fillY = by + 16 + 32 + (int)((maxStamina - energy) * staminaScale);
                var fillHeight = (int)(energy * staminaScale);
                var fillRect = new Rectangle((int)bx + 12, (int)fillY, 24, fillHeight);
                var fillColor = new Color(104, 184, 72);
                batch.Draw(pixel, fillRect, fillColor);

                // Faixa escura de 4px no topo do preenchimento.
                var dark = new Color(
                    Math.Max(0, fillColor.R - 50),
                    Math.Max(0, fillColor.G - 50), fillColor.B);
                batch.Draw(pixel, new Rectangle(fillRect.X, fillRect.Y, 24, 4), dark);
            }

            if (font == null)
                return;

            var hour = 6 + gameMinutes / 60;
            var minute = gameMinutes % 60;
            var suffix = hour >= 12 ? "PM" : "AM";
            var h12 = hour % 12;
            if (h12 == 0) h12 = 12;
            var timeText = h12 + ":" + minute.ToString("00") + " " + suffix;
            var timeSize = font.MeasureString(timeText);
            batch.DrawString(font, timeText,
                new Vector2(ViewportConfig.VirtualWidth - 28 - timeSize.X + ox, 24 + oy), Color.White);
            batch.DrawString(font, "500g",
                new Vector2(ViewportConfig.VirtualWidth - 28 - font.MeasureString("500g").X + ox, 56 + oy),
                new Color(255, 228, 120));
            // drawWithBorder: a esquerda da barra, com Y = by + 64.
            var staminaText = energy + "/" + maxStamina;
            var labelSize = font.MeasureString("999/999");
            batch.DrawString(font, staminaText,
                new Vector2(bx - labelSize.X - 16, by + 64), Color.White);




        }

    }
}
