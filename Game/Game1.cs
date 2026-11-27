using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using LumiaValley.Game.Map;
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
        private int sceneWidth;
        private int sceneHeight;
        private IScreen current;

        public static bool HasExternalGameFolder { get; private set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // Nao impor PreferredBackBuffer: no UWP o MonoGame dimensiona a swap
            // chain pelo SwapChainPanel.
            graphics.SupportedOrientations =
                DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            var portContent = new PortContentManager(Services, "Content");
            Content = portContent;
            HasExternalGameFolder = portContent.HasExternalFolder;
            TouchPanel.EnabledGestures = GestureType.Tap;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Leitor dos mapas xTile (formato tBIN10) usados pela 1.5.6.
            ContentTypeReaderManager.AddTypeCreator(
                "xTile.Pipeline.TideReader, xTile", () => new TideMapReader());

            EnsureSceneTarget();

            Localization.Load(Content);
            ChangeScreen(new TitleScreen());

            base.LoadContent();
        }

        /// <summary>Recria a cena quando o usuario troca qualidade nas opcoes.</summary>
        private void EnsureSceneTarget()
        {
            if (sceneTarget != null &&
                sceneWidth == Settings.SceneWidth &&
                sceneHeight == Settings.SceneHeight)
                return;

            sceneTarget?.Dispose();
            sceneWidth = Settings.SceneWidth;
            sceneHeight = Settings.SceneHeight;
            sceneTarget = new RenderTarget2D(GraphicsDevice, sceneWidth, sceneHeight);
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

                // Usa o PRIMEIro toque valido da fila. O codigo anterior
                // sobrescrevia `tap` a cada gesto e terminava usando um toque
                // antigo/phantom, o que fazia teclas erradas responderem.
                var position = gesture.Position;
                var vp = GraphicsDevice.Viewport;
                var virtualPoint = ViewportConfig.ToVirtual(position, vp.Width, vp.Height);
                tap = current != null ? current.FromVirtual(virtualPoint) : virtualPoint;
                break;
            }

            if (current != null)
            {
                var next = current.Update(gameTime, tap);
                if (next != null && !ReferenceEquals(next, current))
                    ChangeScreen(next);
            }

            base.Update(gameTime);
        }

        /// <summary>Transform da cena em uso, para telas que precisam trocar
        /// o blend state no meio do desenho (ex.: lightmap aditivo).</summary>
        public static Matrix SceneTransform { get; private set; }

        protected override void Draw(GameTime gameTime)
        {
            EnsureSceneTarget();

            // 1. Cena em resolucao configurada (640x360 por padrao).
            GraphicsDevice.SetRenderTarget(sceneTarget);
            GraphicsDevice.Clear(new Color(28, 64, 44));

            var sceneScale = ViewportConfig.GetScale(sceneWidth, sceneHeight);
            var screenTransform = current != null ? current.Transform : Matrix.Identity;
            var transform = screenTransform *
                Matrix.CreateScale(sceneScale.X, sceneScale.Y, 1f);
            SceneTransform = transform;

            spriteBatch.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp);
            current?.Draw(spriteBatch);
            spriteBatch.End();

            // 2. Um unico blit escalado para o backbuffer real do SwapChainPanel.
            // IMPORTANTE: ler o viewport DEPOIS de SetRenderTarget(null).
            GraphicsDevice.SetRenderTarget(null);
            var vp = GraphicsDevice.Viewport;
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(
                sceneTarget,
                new Rectangle(0, 0, vp.Width, vp.Height),
                Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
