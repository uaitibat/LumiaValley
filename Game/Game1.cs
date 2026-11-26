using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using LumiaValley.Game.Screens;

namespace LumiaValley.Game
{
    /// <summary>
    /// Host do port. Nao contem logica de telas: apenas gerencia o viewport,
    /// a cena em 640x360, o blit final e a troca de IScreen.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private RenderTarget2D sceneTarget;
        private IScreen current;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // Nao impor PreferredBackBuffer: no UWP o MonoGame dimensiona a swap
            // chain pelo SwapChainPanel.
            graphics.SupportedOrientations =
                DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            Content.RootDirectory = "Content";
            TouchPanel.EnabledGestures = GestureType.Tap;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sceneTarget = new RenderTarget2D(
                GraphicsDevice,
                ViewportConfig.SceneWidth,
                ViewportConfig.SceneHeight);

            ChangeScreen(new TitleScreen());

            base.LoadContent();
        }

        private void ChangeScreen(IScreen next)
        {
            next?.Load(Content, GraphicsDevice);
            current = next;
        }

        protected override void Update(GameTime gameTime)
        {
            Vector2? tap = null;
            while (TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType != GestureType.Tap)
                    continue;

                var vp = GraphicsDevice.Viewport;
                tap = ViewportConfig.ToVirtual(gesture.Position, vp.Width, vp.Height);
            }

            if (current != null)
            {
                var next = current.Update(gameTime, tap);
                if (next != null && !ReferenceEquals(next, current))
                    ChangeScreen(next);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // 1. Cena em resolucao reduzida (640x360), independente do backbuffer.
            GraphicsDevice.SetRenderTarget(sceneTarget);
            GraphicsDevice.Clear(new Color(28, 64, 44));

            var sceneScale = ViewportConfig.GetScale(
                ViewportConfig.SceneWidth,
                ViewportConfig.SceneHeight);

            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(sceneScale.X, sceneScale.Y, 1f));
            current?.Draw(spriteBatch);
            spriteBatch.End();

            // 2. Um unico blit escalado para o backbuffer real do SwapChainPanel.
            // IMPORTANTE: ler o viewport DEPOIS de SetRenderTarget(null).
            GraphicsDevice.SetRenderTarget(null);
            var vp = GraphicsDevice.Viewport;
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(
                sceneTarget,
                new Rectangle(0, 0, vp.Width, vp.Height),
                Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
