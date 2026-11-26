using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Uma tela do port. Coordenadas de Draw/tap sao sempre virtuais 1280x720.
    /// Update devolve a proxima tela (ou a propria para permanecer).
    /// </summary>
    public interface IScreen
    {
        void Load(ContentManager content, GraphicsDevice device);

        IScreen Update(GameTime gameTime, Vector2? tap);

        void Draw(SpriteBatch spriteBatch);

        /// <summary>Transformacao aplicada pelo proprio screen no espaco virtual.</summary>
        Matrix Transform { get; }

        /// <summary>Converte um toque virtual para as coordenadas usadas no Draw.</summary>
        Vector2 FromVirtual(Vector2 point);
    }
}
