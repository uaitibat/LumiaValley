using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace LumiaValley.Game
{
    /// <summary>
    /// Tela de titulo minima do port: sprites reais da 1.5.6 e selecao por toque.
    /// Nao copia a logica do TitleMenu descompilado; apenas reusa as regioes dos sprites.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private const int ScreenW = 640;
        private const int ScreenH = 360;

        private static readonly Rectangle LogoSource = new Rectangle(0, 0, 400, 187);
        private static readonly Rectangle[] ButtonSources =
        {
            new Rectangle(0, 187, 74, 58),    // New
            new Rectangle(74, 187, 74, 58),   // Load
            new Rectangle(148, 187, 74, 58),  // Co-op
            new Rectangle(222, 187, 74, 58)   // Exit
        };

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D dayBg;
        private Texture2D titleButtons;
        private Texture2D pixel;
        private int selected = -1;
        private bool xnbLoaded;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = ScreenW;
            graphics.PreferredBackBufferHeight = ScreenH;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            Content.RootDirectory = "Content";
            TouchPanel.EnabledGestures = GestureType.Tap;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            try
            {
                dayBg = Content.Load<Texture2D>("LooseSprites/daybg");
                titleButtons = Content.Load<Texture2D>("Minigames/TitleButtons");
                xnbLoaded = dayBg != null && titleButtons != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao carregar XNB: " + ex);
                xnbLoaded = false;
            }

            base.LoadContent();
        }

        private static Rectangle ButtonDest(int index)
        {
            return new Rectangle(284, 124 + index * 60, 72, 56);
        }

        protected override void Update(GameTime gameTime)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType != GestureType.Tap)
                    continue;

                var x = (int)gesture.Position.X;
                var y = (int)gesture.Position.Y;

                for (var i = 0; i < ButtonSources.Length; i++)
                {
                    if (ButtonDest(i).Contains(x, y))
                    {
                        selected = i;
                        break;
                    }
                }
            }

            base.Update(gameTime);
        }

        private void DrawBorder(Rectangle r, Color color)
        {
            spriteBatch.Draw(pixel, new Rectangle(r.X - 2, r.Y - 2, r.Width + 4, 2), color);
            spriteBatch.Draw(pixel, new Rectangle(r.X - 2, r.Bottom, r.Width + 4, 2), color);
            spriteBatch.Draw(pixel, new Rectangle(r.X - 2, r.Y, 2, r.Height), color);
            spriteBatch.Draw(pixel, new Rectangle(r.Right, r.Y, 2, r.Height), color);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(28, 64, 44));

            spriteBatch.Begin();

            if (xnbLoaded)
            {
                spriteBatch.Draw(dayBg, new Rectangle(0, 0, ScreenW, ScreenH), Color.White);
                spriteBatch.Draw(titleButtons, new Rectangle(200, 8, 240, 112), LogoSource, Color.White);

                for (var i = 0; i < ButtonSources.Length; i++)
                {
                    var dest = ButtonDest(i);
                    var tint = i == selected ? Color.White : new Color(190, 190, 190);
                    spriteBatch.Draw(titleButtons, dest, ButtonSources[i], tint);

                    if (i == selected)
                        DrawBorder(dest, Color.Red);
                }
            }
            else
            {
                spriteBatch.Draw(
                    pixel,
                    new Rectangle(ScreenW / 2 - 16, ScreenH / 2 - 16, 32, 32),
                    Color.Wheat);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
