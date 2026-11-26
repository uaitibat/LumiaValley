using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace LumiaValley.Game
{
    /// <summary>
    /// Primeira fatia do port: valida leitura de XNB v5 no UWP ARM.
    /// O marcador vermelho permanece apenas como diagnostico do TouchPanel.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D dayBg;
        private Texture2D pixel;
        private Vector2 marker;
        private bool xnbLoaded;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 360;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            Content.RootDirectory = "Content";
            TouchPanel.EnabledGestures = GestureType.Tap;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            marker = new Vector2(320, 180);

            try
            {
                dayBg = Content.Load<Texture2D>("LooseSprites/daybg");
                xnbLoaded = dayBg != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Falha ao carregar XNB: " + ex);
                xnbLoaded = false;
            }

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.Tap)
                    marker = gesture.Position;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(28, 64, 44));

            spriteBatch.Begin();

            if (xnbLoaded)
            {
                spriteBatch.Draw(dayBg, new Rectangle(0, 0, 640, 360), Color.White);
                spriteBatch.Draw(
                    pixel,
                    new Rectangle((int)marker.X - 8, (int)marker.Y - 8, 16, 16),
                    Color.Red);
            }
            else
            {
                spriteBatch.Draw(
                    pixel,
                    new Rectangle((int)marker.X - 16, (int)marker.Y - 16, 32, 32),
                    Color.Wheat);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
