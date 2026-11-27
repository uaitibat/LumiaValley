using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Home do port, reimplementada a partir das regioes de sprite da 1.5.6.
    /// Sequencia: splash da ConcernedApe -> fade branco -> pausa -> subida da
    /// camera -> swipe do logo -> botoes. O toque no splash aciona a animacao
    /// de surpresa; ele NAO pula o resto da introducao.
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
        private double surprisedMs;
        private bool homeBirdsStarted;
        private readonly bool skipIntro;

        public TitleScreen() : this(false)
        {
        }

        /// <summary>
        /// Usado ao voltar dos menus: monta a home ja assentada, sem repetir
        /// a abertura completa.
        /// </summary>
        public TitleScreen(bool skipIntro)
        {
            this.skipIntro = skipIntro;
        }

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

            totalMs = 0;
            surprisedMs = 0;

            if (skipIntro)
            {
                phase = Phase.Ready;
                phaseMs = 1000;
                viewportY = -1000f;
                buttonsShown = 4;
                homeBirdsStarted = true;
                birdAX = 1280f;
                birdBX = 1360f;
            }
            else
            {
                phase = Phase.Ape;
                phaseMs = 0;
                viewportY = 0;
                buttonsShown = 0;
                homeBirdsStarted = false;
            }
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

        public IScreen Update(GameTime gameTime, Vector2? tap)
        {
            var dt = Math.Min(gameTime.ElapsedGameTime.TotalMilliseconds, 50);
            totalMs += dt;
            phaseMs += dt;

            // Toque no splash nao pula a intro: ele aciona a animacao de
            // surpresa do logo (equivalente ao skipButton da 1.5.6).
            if (tap.HasValue && phase == Phase.Ape &&
                Contains(new Rectangle(379, 258, 249, 201), tap.Value))
            {
                if (surprisedMs <= 0)
                    surprisedMs = 1500;
                else if (surprisedMs > 1)
                    surprisedMs = Math.Max(1, surprisedMs - 500);
            }

            switch (phase)
            {
                case Phase.Ape:
                    if (surprisedMs > 0)
                    {
                        surprisedMs -= dt;
                        if (surprisedMs <= 0)
                        {
                            surprisedMs = 0;
                            Next(Phase.WhiteFade);
                        }
                    }
                    else if (phaseMs >= 5000)
                    {
                        Next(Phase.WhiteFade);
                    }
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
                    if (t >= 1f)
                    {
                        StartHomeBirds();
                        Next(Phase.LogoSwipe);
                    }
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

            // Os passaros voam o tempo todo, mas em Y fixo: a subida da camera
            // nao puxa eles para cima como acontece com o ceu/montanhas.
            birdAX -= 40f * (float)seconds;
            birdBX -= 40f * (float)seconds;
            if (birdAX < -80f) birdAX = 1280f;
            if (birdBX < -80f) birdBX = 1280f;

            if (tap.HasValue && phase == Phase.Ready && buttonsShown == 4)
            {
                // Cada botao executa no primeiro toque.
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
                    // Um unico toque ja executa o botao (sem selecao em duas etapas).
                    switch (hit)
                    {
                        case 0: return new CharacterCreationScreen();
                        case 1: return new LoadGameScreen();
                        case 2: return new CoopScreen();
                        case 3: return new ConfirmExitScreen();
                    }
                }
                else
                {
                    hovered = -1;
                }

                if (Contains(new Rectangle(16, 16, 36, 36), tap.Value))
                {
                    Settings.SoundEnabled = !Settings.SoundEnabled;
                }
                else if (Contains(new Rectangle(1166, 621, 66, 75), tap.Value))
                {
                    return new AboutScreen();
                }
                else if (Contains(new Rectangle(1166, 522, 81, 75), tap.Value))
                {
                    return new OptionsScreen();
                }
                else if (Contains(new Rectangle(16, 664, 180, 36), tap.Value))
                {
                    // Port tipo PortMaster: escolher a pasta com os XNB do jogo.
                    _ = PortFolderPicker.PickAsync();
                }
            }

            return this;
        }

        public Matrix Transform
        {
            get { return Matrix.Identity; }
        }

        public Vector2 FromVirtual(Vector2 point)
        {
            return point;
        }

        private void Next(Phase next)
        {
            phase = next;
            phaseMs = 0;
        }

        /// <summary>
        /// Quando a camera assenta, os passaros da cena de abertura dao lugar
        /// a uma nova passada pela home, entrando pela direita.
        /// </summary>
        private void StartHomeBirds()
        {
            if (homeBirdsStarted)
                return;

            homeBirdsStarted = true;
            birdAX = 1280f;
            birdBX = 1360f;
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
            // O splash do PC e branco, nao preto. O logo da ConcernedApe e
            // montado de DOIS blocos: o "Ape" a direita do centro e o
            // "Concerned" a esquerda. Desenhar só um bloco descentraliza tudo.
            batch.Draw(pixel, new Rectangle(0, 0, 1280, 720), Color.White);
            if (buttons == null)
                return;

            if (phase == Phase.Ape)
            {
                var fadeIn = Math.Min(1f, (float)(phaseMs / 500.0));
                var fadeOut = Math.Min(1f, (float)((5000.0 - phaseMs) / 500.0));
                var alpha = Math.Max(0f, Math.Min(fadeIn, fadeOut));
                var alternate = (int)(phaseMs / 100) % 2 == 0;

                // Bloco direito ("Ape"): 111x60 a 3x, comecando em x=640.
                SafeDraw(batch, buttons,
                    new Rectangle(640, 270, 333, 180),
                    alternate ? new Rectangle(171, 311, 111, 60)
                              : new Rectangle(282, 311, 111, 60),
                    Color.White * alpha);

                // Bloco esquerdo ("Concerned"): 85x69 a 3x, em x=379.
                // Quando clicado, troca pelos frames de surpresa (y=375).
                if (surprisedMs > 0)
                {
                    var frame = (surprisedMs > 800 || surprisedMs < 400) ? 176 : 260;
                    var surpriseAlpha = surprisedMs < 200
                        ? (float)(surprisedMs / 200.0)
                        : 1f;
                    SafeDraw(batch, buttons,
                        new Rectangle(379, 258, 255, 207),
                        new Rectangle(frame, 375, 85, 69),
                        Color.White * Math.Max(0f, Math.Min(1f, surpriseAlpha)));
                }
                else
                {
                    SafeDraw(batch, buttons,
                        new Rectangle(379, 258, 255, 207),
                        alternate ? new Rectangle(0, 306, 85, 69)
                                  : new Rectangle(85, 306, 85, 69),
                        Color.White * alpha);
                }
            }
            else if (phase == Phase.WhiteFade)
            {
                var alpha = Math.Max(0f, Math.Min(1f, 1f - (float)(phaseMs / 2000.0)));
                batch.Draw(pixel, new Rectangle(0, 0, 1280, 720), Color.White * alpha);
            }
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

            // Primeiro plano de selva: aparece com a camera em 0 e desaparece
            // para baixo enquanto a camera sobe (parallax 2x do original).
            if (clouds != null)
            {
                SafeDraw(batch, clouds,
                    new Rectangle(0, 720 - 426 - (int)(py * 2f), 495, 426),
                    new Rectangle(0, 554, 165, 142), Color.White);
                SafeDraw(batch, clouds,
                    new Rectangle(1280 - 366, 720 - 459 - (int)(py * 2f), 366, 459),
                    new Rectangle(390, 543, 122, 153), Color.White);
            }
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

            // O logo nao aparece pronto: ele desce junto com a subida da camera.
            // n = 2 em 720p (ShouldShrinkLogo), mesma formula do original:
            // y = -300*n - (int)(viewportY/3f)*n  ->  -600 em py=0, 66 em py=-1000.
            var logoY = -600 - (int)(viewportY / 3f) * 2;
            SafeDraw(batch, buttons,
                new Rectangle(240, logoY, 800, 374), LogoSource, Color.White);

            if (phase != Phase.LogoSwipe)
                return;

            // Swipe final: uma segunda copia com origem no centro e escala
            // pulsante, como o logoSwipeTimer da 1.5.6.
            var t = (float)Math.Min(phaseMs / 1000.0, 1.0);
            var pulse = 2f + (0.5f - Math.Abs(t - 0.5f)) * 0.1f;
            if (LogoSource.Right <= buttons.Width && LogoSource.Bottom <= buttons.Height)
            {
                batch.Draw(
                    buttons,
                    new Vector2(640, logoY + 93 * 2),
                    LogoSource,
                    Color.White,
                    0f,
                    new Vector2(200f, 93f),
                    pulse,
                    SpriteEffects.None,
                    0f);
            }
        }

        private void DrawButtons(SpriteBatch batch)
        {
            for (var i = 0; i < buttonsShown; i++)
            {
                var src = i == hovered ? ButtonHover[i] : ButtonNormal[i];
                SafeDraw(batch, buttons, ButtonDest(i), src, Color.White);
            }

            // Rodape: Sobre / Idioma / Mudo. Botao de voltar e o quadrado de
            // alternar janela do PC foram removidos de proposito no port.
            SafeDraw(batch, buttons, new Rectangle(1166, 621, 66, 75),
                new Rectangle(8, 458, 22, 25), Color.White);
            SafeDraw(batch, buttons, new Rectangle(1166, 522, 81, 75),
                new Rectangle(52, 458, 27, 25), Color.White);

            if (cursors != null)
            {
                SafeDraw(batch, cursors, new Rectangle(16, 16, 36, 36),
                    !Settings.SoundEnabled ? new Rectangle(137, 384, 9, 9) : new Rectangle(128, 384, 9, 9),
                    Color.White);
            }

            if (font == null)
                return;

            batch.DrawString(font,
                Localization.CS("StringsFromCSFiles:TitleMenu.cs.11740", "Sobre"),
                new Vector2(1168, 696), Color.White);
            batch.DrawString(font,
                Localization.CS("StringsFromCSFiles:TitleMenu.cs.11740", "Idioma"),
                new Vector2(1168, 597), Color.White);

            // Botao da pasta do jogo (appx tipo port).
            var label = Game1.HasExternalGameFolder
                ? "Pasta do jogo: OK" : "Pasta do jogo";
            batch.Draw(pixel, new Rectangle(16, 664, 180, 36),
                new Color(24, 28, 40) * 0.85f);
            batch.DrawString(font, label, new Vector2(24, 672),
                Game1.HasExternalGameFolder ? new Color(140, 220, 130) : Color.White);
        }

        /// <summary>Ceu e logo usados como fundo da criacao de personagem.</summary>
        public void DrawBackdrop(SpriteBatch spriteBatch)
        {
            DrawSky(spriteBatch);
            DrawLogo(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Na pausa antes da subida o jogo ja mostra o ceu com a camera
            // na posicao inicial; o branco so vale para o splash e o fade.
            if (phase == Phase.Ape || phase == Phase.WhiteFade)
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
