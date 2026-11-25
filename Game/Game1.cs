using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace LumiaValley.Game
{
    /// <summary>
    /// Teste de pipeline: MonoGame UWP ARM no Lumia, com backbuffer baixo e touch.
    /// Validado no dispositivo: tap move o marcador.
    /// O backbuffer 640x360 e escalado pelo SwapChainPanel para a tela do dispositivo.
    /// Depois substituir pelo Game1 descompilado (reference/StardewValley/StardewValley/Game1.cs).
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D pixel;
        private Vector2 marker;

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
            spriteBatch.Draw(pixel, new Rectangle((int)marker.X - 16, (int)marker.Y - 16, 32, 32), Color.Wheat);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
