using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Entrada do Novo Jogo. A criacao do save agora passa pela tela de
    /// customizacao de personagem e escolha de fazenda.
    /// </summary>
    public sealed class NewGameScreen : MenuScreen
    {
        private static readonly Rectangle Area = new Rectangle(240, 112, 800, 496);
        private static readonly Rectangle FarmerRow = new Rectangle(290, 220, 700, 58);
        private static readonly Rectangle FarmRow = new Rectangle(290, 300, 700, 58);
        private static readonly Rectangle CreateRow = new Rectangle(290, 400, 700, 64);

        public override IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (BackTapped(tap))
                return Home();

            if (tap.HasValue && Contains(CreateRow, tap.Value))
            {
                return new CharacterCreationScreen();
            }

            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch, Area, "Novo jogo");
            DrawRow(spriteBatch, FarmerRow, "Fazendeiro:  Fazendeiro", false);
            DrawRow(spriteBatch, FarmRow, "Fazenda:  Fazenda", false);
            DrawRow(spriteBatch, CreateRow, "Criar personagem e escolher fazenda", true);
            DrawBack(spriteBatch);

            if (font != null)
            {
                spriteBatch.DrawString(font,
                    "Nome, aparencia e tipo de fazenda antes de comecar.",
                    new Vector2(292, 492), Color.White * 0.8f);
            }
        }
    }
}
