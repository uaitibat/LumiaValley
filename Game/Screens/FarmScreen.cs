using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>Prototipo de fazenda da Fase 0, agora como uma tela.</summary>
    public sealed class FarmScreen : IScreen
    {
        private readonly FarmScene scene = new FarmScene();

        public void Load(ContentManager content, GraphicsDevice device)
        {
            scene.Load(content, device);
        }

        public IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (tap.HasValue)
                scene.OnTap(tap.Value);

            scene.Update(gameTime);
            return this;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            scene.Draw(spriteBatch);
        }
    }
}
