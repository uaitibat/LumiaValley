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
        private static readonly Rectangle UpKey = new Rectangle(168, 484, 72, 72);
        private static readonly Rectangle DownKey = new Rectangle(168, 628, 72, 72);
        private static readonly Rectangle LeftKey = new Rectangle(96, 556, 72, 72);
        private static readonly Rectangle RightKey = new Rectangle(240, 556, 72, 72);

        private Texture2D pixel;
        private Texture2D farmer;
        private Texture2D cursors;
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

            farmer = content.Load<Texture2D>("Characters/Farmer/farmer_base");
            try { cursors = content.Load<Texture2D>("LooseSprites/Cursors.pt-BR"); }
            catch { cursors = null; }
            try { font = content.Load<SpriteFont>("Fonts/SmallFont.pt-BR"); }
            catch { font = null; }

            try { map = content.Load<TileMap>("Maps/Farm"); }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao carregar Maps/Farm: " + ex.Message);
                map = null;
            }

            if (map != null)
            {
                foreach (var sheet in map.TileSheets)
                {
                    if (string.IsNullOrEmpty(sheet.ImageSource))
                        continue;
                    try
                    {
                        sheets[sheet.Id] = content.Load<Texture2D>("Maps/" + sheet.ImageSource);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Tilesheet " + sheet.ImageSource + ": " + ex.Message);
                    }
                }
            }

            Reset();
        }

        public void Reset()
        {
            var worldWidth = map != null ? map.Width * 16 : ViewportConfig.VirtualWidth;
            var worldHeight = map != null ? map.Height * 16 : ViewportConfig.VirtualHeight;
            player = new Vector2(worldWidth / 2f, worldHeight / 2f);
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
            if (Contains(UpKey, screenPoint)) { direction = new Vector2(0, -1); hasTarget = false; return; }
            if (Contains(DownKey, screenPoint)) { direction = new Vector2(0, 1); hasTarget = false; return; }
            if (Contains(LeftKey, screenPoint)) { direction = new Vector2(-1, 0); hasTarget = false; return; }
            if (Contains(RightKey, screenPoint)) { direction = new Vector2(1, 0); hasTarget = false; return; }

            target = screenPoint + camera;
            hasTarget = true;
            direction = Vector2.Zero;
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
            if (direction != Vector2.Zero)
            {
                var move = direction.Length() > 1f ? Vector2.Normalize(direction) : direction;
                player += move * speed * dt;
                hasTarget = false;
            }
            else if (hasTarget)
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
            var maxX = map.Width * 16f;
            var maxY = map.Height * 16f;
            player.X = MathHelper.Clamp(player.X, 32f, maxX - 32f);
            player.Y = MathHelper.Clamp(player.Y, 48f, maxY - 16f);
        }

        private void UpdateCamera()
        {
            var worldWidth = map != null ? map.Width * 16 : ViewportConfig.VirtualWidth;
            var worldHeight = map != null ? map.Height * 16 : ViewportConfig.VirtualHeight;
            camera.X = MathHelper.Clamp(player.X - ViewportConfig.VirtualWidth / 2f,
                0f, Math.Max(0f, worldWidth - ViewportConfig.VirtualWidth));
            camera.Y = MathHelper.Clamp(player.Y - ViewportConfig.VirtualHeight / 2f,
                0f, Math.Max(0f, worldHeight - ViewportConfig.VirtualHeight));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawMap(spriteBatch);
            DrawFarmer(spriteBatch);
            DrawHud(spriteBatch);
            DrawDPad(spriteBatch);
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

                var tileW = layer.TileWidth;
                var tileH = layer.TileHeight;
                var startX = Math.Max(0, (int)(camera.X / tileW) - 1);
                var startY = Math.Max(0, (int)(camera.Y / tileH) - 1);
                var endX = Math.Min(layer.Width, (int)((camera.X + ViewportConfig.VirtualWidth) / tileW) + 2);
                var endY = Math.Min(layer.Height, (int)((camera.Y + ViewportConfig.VirtualHeight) / tileH) + 2);

                for (var y = startY; y < endY; y++)
                {
                    for (var x = startX; x < endX; x++)
                    {
                        var tile = layer.Get(x, y);
                        if (tile == null || tile.Sheet == null)
                            continue;
                        Texture2D texture;
                        if (!sheets.TryGetValue(tile.Sheet.Id, out texture) || texture == null)
                            continue;

                        var col = tile.Index % tile.Sheet.SheetWidth;
                        var row = tile.Index / tile.Sheet.SheetWidth;
                        var src = new Rectangle(
                            col * (tile.Sheet.TileWidth + tile.Sheet.SpacingX) + tile.Sheet.MarginX,
                            row * (tile.Sheet.TileHeight + tile.Sheet.SpacingY) + tile.Sheet.MarginY,
                            tile.Sheet.TileWidth, tile.Sheet.TileHeight);
                        if (src.X < 0 || src.Y < 0 || src.Right > texture.Width || src.Bottom > texture.Height)
                            continue;

                        batch.Draw(texture, new Rectangle(x * tileW, y * tileH, tileW, tileH), src, Color.White);
                    }
                }
            }
        }

        private void DrawFarmer(SpriteBatch batch)
        {
            if (farmer == null)
                return;
            // 16x32 * 4 = 64x128, com os pes no ponto do jogador.
            batch.Draw(farmer,
                new Rectangle((int)player.X - 32, (int)player.Y - 112, 64, 128),
                new Rectangle(0, 0, 16, 32), Color.White);
        }

        private void DrawHud(SpriteBatch batch)
        {
            // Barra de energia.
            batch.Draw(pixel, new Rectangle(24, 22, 208, 28), Color.Black * 0.6f);
            batch.Draw(pixel, new Rectangle(28, 26, (int)(200 * energy / 100f), 20), new Color(104, 184, 72));

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
                new Vector2(ViewportConfig.VirtualWidth - 28 - timeSize.X, 24), Color.White);
            batch.DrawString(font, "500g",
                new Vector2(ViewportConfig.VirtualWidth - 28 - font.MeasureString("500g").X, 56),
                new Color(255, 228, 120));

            if (slot != null && !string.IsNullOrEmpty(slot.FarmerName))
                batch.DrawString(font, slot.FarmerName + " - " + slot.FarmName, new Vector2(24, 60), Color.White);
        }

        private void DrawDPad(SpriteBatch batch)
        {
            var keys = new[] { UpKey, DownKey, LeftKey, RightKey };
            foreach (var key in keys)
            {
                batch.Draw(pixel, key, Color.Black * 0.55f);
                batch.Draw(pixel, new Rectangle(key.X + 2, key.Y + 2, key.Width - 4, 2),
                    new Color(126, 170, 92));
            }
            if (cursors == null)
                return;

            batch.Draw(cursors, UpKey, new Rectangle(0, 320, 64, 64), Color.White);
            batch.Draw(cursors, DownKey, new Rectangle(0, 192, 64, 64), Color.White);
            batch.Draw(cursors, LeftKey, new Rectangle(0, 256, 64, 64), Color.White);
            batch.Draw(cursors, RightKey, new Rectangle(64, 256, 64, 64), Color.White);
        }

        private static bool Contains(Rectangle r, Vector2 p)
        {
            return r.Contains((int)p.X, (int)p.Y);
        }
    }
}
