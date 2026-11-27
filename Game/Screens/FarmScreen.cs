using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>Prototipo de fazenda da Fase 0, agora como tela jogavel.</summary>
    public sealed class FarmScreen : IScreen
    {
        private readonly FarmScene scene;
        private readonly SaveSlot slot;
        private SpriteFont font;

        public FarmScreen(SaveSlot slot = null)
        {
            this.slot = slot;
            scene = new FarmScene(slot);
        }

        public void Load(ContentManager content, GraphicsDevice device)
        {
            scene.Load(content, device);
            try
            {
                font = content.Load<SpriteFont>("Fonts/SmallFont.pt-BR");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Fonte indisponivel: " + ex.Message);
            }
        }

        public Matrix Transform
        {
            get { return scene.Transform; }
        }

        public Vector2 FromVirtual(Vector2 point)
        {
            return point;
        }

        public IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (tap.HasValue)
            {
                if (scene.HasDialog)
                    scene.HandleDialogTap(tap.Value);
                else
                    scene.OnTap(tap.Value);
            }

            scene.Update(gameTime);
            return this;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            scene.Draw(spriteBatch);

            if (font == null)
                return;

            var label = slot == null
                ? "Fazenda de teste"
                : slot.FarmerName + "  -  " + slot.FarmName +
                  "  [" + FarmTypes.Name(slot.FarmType) + "]";

            spriteBatch.DrawString(font, label, new Vector2(20, 16), Color.White);
        }
    }
}
