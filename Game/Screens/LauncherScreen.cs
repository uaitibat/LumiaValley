using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Home vertical do appx "tipo port": mostra a foto do usuario como
    /// fundo e dois botoes — escolher a pasta do jogo e iniciar. O jogo em si
    /// continua em 1280x720 paisagem (ChangeScreen troca a orientacao).
    /// </summary>
    public sealed class LauncherScreen : IScreen
    {
        private Texture2D background;
        private Texture2D pixel;
        private SpriteFont font;
        private string status;

        public LauncherScreen()
        {
            var count = PortContentManager.GetAssetCount();
            status = count > 0
                ? "Pasta do jogo: " + count + " XNB carregados. Pode iniciar."
                : "Toque em \"Pasta do jogo\" para escolher onde estao os XNB.";
        }

        public void Load(ContentManager content, GraphicsDevice device)
        {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] { Color.White });
            background = LoadPackageImage(device, "Assets/LauncherBackground.jpg");
            // A fonte foi embutida em Assets/Fonts (fora do Content externo).
            try
            {
                var assetFonts = new ContentManager(content.ServiceProvider, "Assets");
                font = assetFonts.Load<SpriteFont>("Fonts/SmallFont.pt-BR");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Fonte: " + ex.Message);
                font = null;
            }
        }

        /// <summary>Carrega um JPG do pacote pelo FromStream do MonoGame
        /// (sincrono, sem BitmapDecoder/GetResult que trava no startup).</summary>
        private static Texture2D LoadPackageImage(GraphicsDevice device, string path)
        {
            try
            {
                var uri = new Uri("ms-appx:///" + path.Replace('\\', '/'));
                var file = Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri)
                    .AsTask().GetAwaiter().GetResult();
                using (var stream = file.OpenStreamForReadAsync().GetAwaiter().GetResult())
                {
                    return Texture2D.FromStream(device, stream);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Foto: " + ex.Message);
                return null;
            }
        }

        public Matrix Transform { get { return Matrix.Identity; } }
        public Vector2 FromVirtual(Vector2 point) { return point; }

        public IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (!tap.HasValue)
                return this;
            var p = tap.Value;
            var w = ViewportConfig.VirtualWidth;

            // Botao "Pasta do jogo".
            if (p.X > 40 && p.X < w - 40 && p.Y > 880 && p.Y < 980)
            {
                status = "Lendo pasta do jogo...";
                _ = PickAndReport();
            }
            // Botao "Iniciar" so com pasta validada.
            else if (p.X > 40 && p.X < w - 40 && p.Y > 1010 && p.Y < 1110)
            {
                if (PortContentManager.GetAssetCount() > 0)
                    return new TitleScreen();
                status = "Escolha primeiro a pasta do jogo.";
            }
            return this;
        }

        private async Task PickAndReport()
        {
            var ok = await PortFolderPicker.PickAsync();
            var count = PortContentManager.GetAssetCount();
            status = ok
                ? "Pasta do jogo: " + count + " XNB carregados. Pode iniciar."
                : "Pasta invalida: " + count + " XNB achados. Selecione a pasta Content.";
        }

        public void Draw(SpriteBatch batch)
        {
            var w = ViewportConfig.VirtualWidth;
            var h = ViewportConfig.VirtualHeight;

            if (background != null)
            {
                var scale = Math.Max((float)w / background.Width, (float)h / background.Height);
                var dw = background.Width * scale;
                var dh = background.Height * scale;
                batch.Draw(background,
                    new Rectangle((int)((w - dw) / 2), (int)((h - dh) / 2), (int)dw, (int)dh),
                    Color.White);
            }
            else
            {
                batch.Draw(pixel, new Rectangle(0, 0, w, h), new Color(20, 24, 34));
            }

            // Titulo com sombra direto sobre a foto (sem faixa que corta).
            if (font != null)
            {
                batch.DrawString(font, "LumiaValley V1.1", new Vector2(42, 62), Color.Black * 0.8f);
                batch.DrawString(font, "LumiaValley V1.1", new Vector2(40, 60), Color.White);
                batch.DrawString(font, status, new Vector2(42, 152), Color.Black * 0.8f);
                batch.DrawString(font, status, new Vector2(40, 150),
                    new Color(230, 230, 210));
            }

            DrawButton(batch, 880, Game1.HasExternalGameFolder ? "Pasta do jogo: OK" : "Pasta do jogo",
                new Color(38, 70, 52));
            DrawButton(batch, 1010, "Iniciar jogo", new Color(68, 52, 30));
        }

        private void DrawButton(SpriteBatch batch, int y, string text, Color color)
        {
            var w = ViewportConfig.VirtualWidth;
            batch.Draw(pixel, new Rectangle(40, y, w - 80, 100), color * 0.9f);
            batch.Draw(pixel, new Rectangle(40, y, w - 80, 4), new Color(220, 196, 128));
            if (font != null)
                batch.DrawString(font, text, new Vector2(70, y + 34), Color.White);
        }
    }
}
