using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Home do port, reimplementada a partir das regioes de sprite da 1.5.6.
    /// Sequencia: splash da ConcernedApe -> fade branco -> pausa -> subida da
    /// camera -> swipe do logo -> botoes. Um toque pula a introducao.
    /// </summary>
    public sealed class TitleScreen : IScreen
    {
        private enum Phase { Ape, WhiteFade, Pause, CameraRise, LogoSwipe, Ready }

        private static readonly Rectangle LogoSource = new Rectangle(0, 0, 400, 187);
        private static readonly Rectangle[] ButtonNormal =
        {
            new Rectangle(0, 187, 74, 58),
            new Rectangle(74, 187, 74, 58),
            new Rectangle(148, 187, 74, 58),
            new Rectangle(222, 187, 74, 58)
        };
        private static readonly Rectangle[] ButtonHover =
        {
            new Rectangle(0, 245, 74, 58),
            new Rectangle(74, 245, 74, 58),
            new Rectangle(148, 245, 74, 58),
            new Rectangle(222, 245, 74, 58)
        };

        private static readonly Rectangle[] SmallCloudSources =
        {
            new Rectangle(152, 447, 123, 55),
            new Rectangle(0, 471, 149, 66),
            new Rectangle(410, 467, 63, 37)
        };

        private Texture2D pixel;
        private Texture2D buttons;
        private Texture2D clouds;
        private Texture2D cursors;
        private SpriteFont font;

        private Phase phase;
        private double phaseMs;
        private double totalMs;
        private float viewportY;
        private int buttonsShown;
        private int hovered = -1;
        private bool muted;

        private readonly float[] smallCloudX = new float[16];
        private float bigCloudX = 960f;
        private float birdAX = 1070f;
        private float birdBX = 1160f;

        public void Load(ContentManager content, GraphicsDevice device)
        {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] { Color.White });

            buttons = LoadFirst(content, "Minigames/TitleButtons.pt-BR", "Minigames/TitleButtons");
            clouds = LoadFirst(content, "Minigames/Clouds");
            cursors = LoadFirst(content, "LooseSprites/Cursors.pt-BR", "LooseSprites/Cursors");
            font = LoadFirstFont(content, "Fonts/SmallFont.pt-BR", "Fonts/SmallFont");

            Localization.Load(content);

            for (var i = 0; i < smallCloudX.Length; i++)
                smallCloudX[i] = 80f + i * 96f;

            phase = Phase.Ape;
            phaseMs = 0;
            totalMs = 0;
            viewportY = 0;
            buttonsShown = 0;
        }

        private static Texture2D LoadFirst(ContentManager content, params string[] assets)
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

        private static SpriteFont LoadFirstFont(ContentManager content, params string[] assets)
        {
            foreach (var asset in assets)
            {
                try
                {
                    var result = content.Load<SpriteFont>(asset);
                    if (result != null)
                        return result;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Fonte indisponivel " + asset + ": " + ex.Message);
                }
            }
            return null;
        }

        private static Rectangle ButtonDest(int index)
        {
            return new Rectangle(160 + index * 246, 522, 222, 174);
        }

        private static bool Contains(Rectangle r, Vector2 p)
        {
            return r.Contains((int)p.X, (int)p.Y);
        }

        private void SkipIntro()
        {
            phase = Phase.Ready;
            phaseMs = 1000;
            viewportY = -1000f;
            buttonsShown = 4;
        }

        public IScreen Update(GameTime gameTime, Vector2? tap)
        {
            var dt = Math.Min(gameTime.ElapsedGameTime.TotalMilliseconds, 50);
            totalMs += dt;
            phaseMs += dt;

            if (tap.HasValue && phase != Phase.Ready)
                SkipIntro();

            switch (phase)
            {
                case Phase.Ape:
                    if (phaseMs >= 5000) Next(Phase.WhiteFade);
                    break;
                case Phase.WhiteFade:
                    if (phaseMs >= 4000) Next(Phase.Pause);
                    break;
                case Phase.Pause:
                    if (phaseMs >= 3500) Next(Phase.CameraRise);
                    break;
                case Phase.CameraRise:
                {
                    var t = (float)Math.Min(phaseMs / 2500.0, 1.0);
                    viewportY = -1000f * t * t;
                    if (t >= 1f) Next(Phase.LogoSwipe);
                    break;
                }
                case Phase.LogoSwipe:
                    if (phaseMs >= 1000) Next(Phase.Ready);
                    break;
                case Phase.Ready:
                    if (buttonsShown < 4 && phaseMs >= buttonsShown * 200)
                        buttonsShown++;
                    break;
            }

            // Parallax leve, em pixels por segundo (equivalente ao movimento
            // por frame da 1.5.6, mas independente da taxa de quadros).
            var seconds = dt / 1000.0;
            bigCloudX -= 6f * (float)seconds;
            if (bigCloudX < -1536f) bigCloudX = 1280f;

            for (var i = 0; i < smallCloudX.Length; i++)
            {
                smallCloudX[i] -= 18f * (float)seconds;
                if (smallCloudX[i] < -447f) smallCloudX[i] = 1280f;
            }

            birdAX -= 40f * (float)seconds;
            birdBX -= 40f * (float)seconds;
            if (birdAX < -80f) birdAX = 1280f;
            if (birdBX < -80f) birdBX = 1280f;

            if (tap.HasValue && phase == Phase.Ready && buttonsShown == 4)
            {
                // Seleciona no primeiro toque e executa no segundo.
                var hit = -1;
                for (var i = 0; i < 4; i++)
                {
                    if (Contains(ButtonDest(i), tap.Value))
                    {
                        hit = i;
                        break;
                    }
                }

                if (hit >= 0)
                {
                    if (hovered == hit)
                    {
                        if (hit == 0 || hit == 1)
                            return new FarmScreen();
                    }
                    else
                    {
                        hovered = hit;
                    }
                }
                else if (Contains(new Rectangle(16, 16, 36, 36), tap.Value))
                {
                    muted = !muted;
                }
            }

            return this;
        }

        private void Next(Phase next)
        {
            phase = next;
            phaseMs = 0;
        }

        private static void SafeDraw(
            SpriteBatch batch, Texture2D texture, Rectangle dest, Rectangle source, Color color)
        {
            if (texture == null)
                return;
            if (source.X < 0 || source.Y < 0 ||
                source.Right > texture.Width || source.Bottom > texture.Height)
                return;
            batch.Draw(texture, dest, source, color);
        }

        private void DrawIntro(SpriteBatch batch)
        {
            batch.Draw(pixel, new Rectangle(0, 0, 1280, 720), Color.Black);
            if (buttons == null)
                return;

            var alpha = 1f;
            if (phase == Phase.WhiteFade)
                alpha = Math.Max(0f, 1f - (float)(phaseMs / 2000.0));
            else if (phase == Phase.Ape)
            {
                var fadeIn = Math.Min(1f, (float)(phaseMs / 500.0));
                var fadeOut = Math.Min(1f, (float)((5000.0 - phaseMs) / 500.0));
                alpha = Math.Min(fadeIn, fadeOut);
            }

            var frame = ((int)(phaseMs / 100) % 2 == 0)
                ? new Rectangle(171, 311, 111, 60)
                : new Rectangle(282, 311, 111, 60);
            SafeDraw(batch, buttons, new Rectangle(640, 270, 333, 180), frame,
                Color.White * alpha);

            if (phase == Phase.WhiteFade)
                batch.Draw(pixel, new Rectangle(0, 0, 1280, 720),
                    Color.White * Math.Max(0f, 1f - (float)(phaseMs / 2000.0)));
        }

        private void DrawSky(SpriteBatch batch)
        {
            var py = viewportY;

            // Ceu esticado a partir da coluna de 1 pixel do Cursors. Se a regiao
            // nao existir na textura, usa um azul solido como fallback.
            var skySource = new Rectangle(703, 1912, 1, 264);
            if (cursors == null || skySource.Right > cursors.Width || skySource.Bottom > cursors.Height)
            {
                batch.Draw(pixel, new Rectangle(0, 0, 1280, 720), new Color(116, 186, 214));
            }
            else
            {
                SafeDraw(batch, cursors,
                    new Rectangle(0, -900 - (int)(py * 0.66f), 1280, 1260),
                    skySource, Color.White);
            }

            // Montanhas ao fundo e cadeia de morros (o segundo par fica fora
            // da viewport final, mas aparece durante a subida da camera).
            SafeDraw(batch, cursors,
                new Rectangle(-30, -1080 - (int)(py * 0.66f), 2552, 780),
                new Rectangle(0, 1453, 638, 195), Color.White * 0.85f);
            SafeDraw(batch, cursors,
                new Rectangle(-90, 720 - 474 - (int)(py * 0.66f), 1917, 444),
                new Rectangle(0, 886, 639, 148), Color.White * 0.9f);
            SafeDraw(batch, cursors,
                new Rectangle(0, 720 - 444 - (int)py, 1917, 444),
                new Rectangle(0, 737, 639, 148), Color.White);

            DrawClouds(batch, py);
            DrawBirds(batch);
        }

        private void DrawClouds(SpriteBatch batch, float py)
        {
            if (clouds == null)
                return;

            SafeDraw(batch, clouds,
                new Rectangle((int)bigCloudX, 720 - 750 - (int)(py * 0.5f), 1536, 1011),
                new Rectangle(0, 0, 512, 337), Color.White * 0.95f);

            for (var i = 0; i < smallCloudX.Length; i++)
            {
                var src = SmallCloudSources[i % SmallCloudSources.Length];
                SafeDraw(batch, clouds,
                    new Rectangle((int)smallCloudX[i],
                        720 - 900 - i * 36 - (int)(py * 0.5f),
                        src.Width * 3, src.Height * 3),
                    src, Color.White * 0.9f);
            }
        }

        private void DrawBirds(SpriteBatch batch)
        {
            if (buttons == null)
                return;

            var frame = (int)((totalMs / 100) % 4);
            var src = new Rectangle(296 + frame * 26, 227, 26, 21);
            SafeDraw(batch, buttons, new Rectangle((int)birdAX, 330, 78, 63), src, Color.White);
            SafeDraw(batch, buttons, new Rectangle((int)birdBX, 360, 78, 63), src, Color.White);
        }

        private void DrawLogo(SpriteBatch batch)
        {
            if (buttons == null)
                return;

            var scale = 2f;
            if (phase == Phase.LogoSwipe)
            {
                var t = (float)Math.Min(phaseMs / 1000.0, 1.0);
                scale += (0.5f - Math.Abs(t - 0.5f)) * 0.1f;
            }

            var width = (int)(LogoSource.Width * scale);
            var height = (int)(LogoSource.Height * scale);
            SafeDraw(batch, buttons,
                new Rectangle(640 - width / 2, 66, width, height), LogoSource, Color.White);
        }

        private void DrawButtons(SpriteBatch batch)
        {
            for (var i = 0; i < buttonsShown; i++)
            {
                var src = i == hovered ? ButtonHover[i] : ButtonNormal[i];
                SafeDraw(batch, buttons, ButtonDest(i), src, Color.White);
            }

            // Rodape: Voltar / Sobre / Idioma / Mudo / Janela.
            SafeDraw(batch, buttons, new Rectangle(1034, 615, 198, 81),
                new Rectangle(296, 252, 66, 27), Color.White);
            SafeDraw(batch, buttons, new Rectangle(1166, 621, 66, 75),
                new Rectangle(8, 458, 22, 25), Color.White);
            SafeDraw(batch, buttons, new Rectangle(1166, 522, 81, 75),
                new Rectangle(52, 458, 27, 25), Color.White);

            if (cursors != null)
            {
                SafeDraw(batch, cursors, new Rectangle(16, 16, 36, 36),
                    muted ? new Rectangle(137, 384, 9, 9) : new Rectangle(128, 384, 9, 9),
                    Color.White);
                SafeDraw(batch, cursors, new Rectangle(1228, 16, 36, 36),
                    new Rectangle(146, 384, 9, 9), Color.White);
            }

            if (font == null)
                return;

            batch.DrawString(font,
                Localization.CS("StringsFromCSFiles:TitleMenu.cs.11739", "Voltar"),
                new Vector2(1046, 692), Color.White);
            batch.DrawString(font,
                Localization.CS("StringsFromCSFiles:TitleMenu.cs.11740", "Sobre"),
                new Vector2(1168, 696), Color.White);
            batch.DrawString(font,
                Localization.CS("StringsFromCSFiles:TitleMenu.cs.11740", "Idioma"),
                new Vector2(1168, 597), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (phase == Phase.Ape || phase == Phase.WhiteFade || phase == Phase.Pause)
            {
                DrawIntro(spriteBatch);
                return;
            }

            DrawSky(spriteBatch);
            DrawLogo(spriteBatch);

            if (phase == Phase.Ready)
                DrawButtons(spriteBatch);
        }
    }
}
