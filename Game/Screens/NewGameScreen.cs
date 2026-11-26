using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Confirmacao do Novo Jogo. A customizacao de personagem/fazenda fica para
    /// a Fase 3; aqui o save inicial ja e criado no formato proprio do port.
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
                SaveStore.Create("Fazendeiro", "Fazenda");
                return new FarmScreen();
            }

            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch, Area, "Novo jogo");
            DrawRow(spriteBatch, FarmerRow, "Fazendeiro:  Fazendeiro", false);
            DrawRow(spriteBatch, FarmRow, "Fazenda:  Fazenda", false);
            DrawRow(spriteBatch, CreateRow, "Criar save e entrar na fazenda", true);
            DrawBack(spriteBatch);

            if (font != null)
            {
                spriteBatch.DrawString(font,
                    "Personalizacao de personagem e fazenda: proxima fase.",
                    new Vector2(292, 492), Color.White * 0.8f);
            }
        }
    }
}
