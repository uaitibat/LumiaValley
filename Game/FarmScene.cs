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
        private Texture2D farmerBase;
        private Texture2D hairSheet;
        private Texture2D shirtSheet;
        private Texture2D pantSheet;
        private Texture2D accessorySheet;
        private Texture2D skinSheet;
        private SpriteFont font;
        private TileMap map;
        private readonly Dictionary<string, Texture2D> sheets = new Dictionary<string, Texture2D>();

        private Vector2 player;
        private Vector2 target;
        private bool hasTarget;
        private Vector2 direction;
        private Vector2 camera;

        private readonly SaveSlot slot;
        private double elapsedMs;
        private int gameMinutes = 360;
        private int energy = 100;

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

        public void Reset()
        {
            // O jogo comeca com o fazendeiro acordando na cama do FarmHouse.
            player = new Vector2(6 * 64f, 3 * 64f);
            target = player;
            hasTarget = false;
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
                }
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
            DrawMap(spriteBatch);
            DrawFurniture(spriteBatch);
            DrawFarmer(spriteBatch);
            DrawHud(spriteBatch);
        }

        private void DrawMap(SpriteBatch batch)
        {
            if (map == null)
            {
                batch.Draw(pixel, new Rectangle(0, 0, ViewportConfig.VirtualWidth,
                    ViewportConfig.VirtualHeight), new Color(74, 132, 62));
                return;
            }

            foreach (var layer in map.Layers)
            {
                if (layer == null || !layer.Visible || layer.Tiles == null)
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

        /// <summary>
        /// Moveis que no xTile vem de tiles com a sheet errada no nosso
        /// parser. Usamos os indices corretos do townInterior (cama) para
        /// sobrepor as brasoes do walls_and_floors.
        /// </summary>
        private void DrawFurniture(SpriteBatch batch)
        {
            Texture2D indoor;
            if (!sheets.TryGetValue("indoor", out indoor) || indoor == null)
                return;

            // Cama: travesseiro (16) na fileira 2, colcha (352/353) na 3,
            // rodape (336/337) na 4.
            for (var x = 1; x <= 10; x++)
            {
                DrawFurnitureTile(batch, indoor, 16, x, 2);
                DrawFurnitureTile(batch, indoor, x % 2 == 0 ? 352 : 353, x, 3);
            }

            // Faixas da frente (camada Front do xTile): rodape da cama,
            // tapete e moveis. Indices do townInterior lidos do tBIN.
            var front = new[]
            {
                160, 161, 162, -1, 163, 165, 165, 165, 165, 165, 166, 167,
                96, 165, 130
            };
            for (var i = 0; i < front.Length; i++)
            {
                if (front[i] < 0)
                    continue;
                var x = i < 12 ? i : i - 12;
                var y = i < 12 ? 10 : 11;
                DrawFurnitureTile(batch, indoor, front[i], x, y);
            }
        }

        private static void DrawFurnitureTile(SpriteBatch batch, Texture2D tex, int index, int tx, int ty)
        {
            const int sheetWidth = 32;
            var src = new Rectangle(index % sheetWidth * 16, index / sheetWidth * 16, 16, 16);
            if (src.Right > tex.Width || src.Bottom > tex.Height)
                return;
            batch.Draw(tex, new Rectangle(tx * 64, ty * 64, 64, 64), src, Color.White);
        }

        /// <summary>Mesmas camadas do preview da criacao: corpo, calca, camisa,
        /// cabelo, braco e acessorio. Escala 4 = 64x128 no espaco do mundo.
        /// </summary>
        private void DrawFarmer(SpriteBatch batch)
        {
            if (farmerBase == null)
                return;

            var pos = new Vector2((float)Math.Floor(player.X - 32), (float)Math.Floor(player.Y - 112));
            var male = slot == null || slot.IsMale;
            // Tom fixo da 1.5.6; a leitura do XNB de skinColors no UWP ARM
            // devolve valores inconsistentes, entao usamos o padrao.
            var skin = new Color(224, 172, 140);
            var hairColor = new Color(110, 58, 30);
            var shirtColor = new Color(88, 110, 170);
            var pantsColor = new Color(64, 86, 150);
            // Corpo (facing 2 = olhando para a camara).
            DrawLayer(batch, farmerBase, pos, new Rectangle(0, 0, 16, 32), skin);

            if (pantSheet != null && slot != null)
            {
                var style = slot.Pants % 20;
                DrawLayer(batch, pantSheet, pos,
                    new Rectangle(style % 10 * 192 + (male ? 0 : 96), style / 10 * 688, 16, 32),
                    pantsColor);
            }

            if (shirtSheet != null && slot != null && slot.Shirt >= 0 && slot.Shirt < 112)
            {
                var sx = slot.Shirt * 8 % 128;
                var sy = slot.Shirt * 8 / 128 * 32;
                var sp = pos + new Vector2(16, 56);
                DrawLayer(batch, shirtSheet, sp, new Rectangle(sx, sy, 8, 8), Color.White);
                DrawLayer(batch, shirtSheet, sp, new Rectangle(sx + 128, sy, 8, 8), shirtColor);
            }

            if (accessorySheet != null && slot != null && slot.Accessory >= 0)
            {
                var acc = slot.Accessory % 20;
                DrawLayer(batch, accessorySheet, pos,
                    new Rectangle(acc * 16 % 128, acc * 16 / 128 * 32, 16, 16), Color.White);
            }

            if (hairSheet != null && slot != null)
            {
                var hair = slot.Hair % 56;
                DrawLayer(batch, hairSheet, pos,
                    new Rectangle(hair % 8 * 16, hair / 8 * 96, 16, 32), hairColor);
            }

            // Braco direito por cima do cabelo.
            DrawLayer(batch, farmerBase, pos, new Rectangle(96, 0, 16, 32), skin);
        }

        private static void DrawLayer(SpriteBatch batch, Texture2D texture, Vector2 pos,
            Rectangle source, Color color)
        {
            if (texture == null || source.X < 0 || source.Y < 0 ||
                source.Right > texture.Width || source.Bottom > texture.Height)
                return;
            batch.Draw(texture, pos, source, color, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
        }

        private void DrawHud(SpriteBatch batch)
        {
            // HUD usa coordenadas de tela: somamos a camera para compensar
            // a transformacao do mundo aplicada pelo FarmScreen.
            var ox = (int)camera.X;
            var oy = (int)camera.Y;

            // Barra de energia.
            batch.Draw(pixel, new Rectangle(24 + ox, 22 + oy, 208, 28), Color.Black * 0.6f);
            batch.Draw(pixel, new Rectangle(28 + ox, 26 + oy, (int)(200 * energy / 100f), 20), new Color(104, 184, 72));

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

            if (slot != null && !string.IsNullOrEmpty(slot.FarmerName))
                batch.DrawString(font, slot.FarmerName + " - " + slot.FarmName,
                    new Vector2(24 + ox, 60 + oy), Color.White);

            // Identificacao do build, para validacao no aparelho.
            batch.DrawString(font, "build " + BuildInfo.Hash,
                new Vector2(24 + ox, 84 + oy), new Color(180, 180, 180));


        }

    }
}
