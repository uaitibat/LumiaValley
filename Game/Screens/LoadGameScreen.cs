using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>Lista os saves proprios do port e entra na fazenda do slot tocado.</summary>
    public sealed class LoadGameScreen : MenuScreen
    {
        private const int MaxSlots = 5;
        private static readonly Rectangle Area = new Rectangle(240, 96, 800, 528);
        private static readonly Rectangle TitleRow = new Rectangle(290, 176, 700, 58);

        private List<SaveSlot> slots = new List<SaveSlot>();

        public override void Load(ContentManager content, GraphicsDevice device)
        {
            base.Load(content, device);
            slots = SaveStore.List();
        }

        private static Rectangle SlotRow(int index)
        {
            return new Rectangle(290, 256 + index * 68, 700, 56);
        }

        public override IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (BackTapped(tap))
                return Home();

            if (!tap.HasValue)
                return this;

            for (var i = 0; i < slots.Count && i < MaxSlots; i++)
            {
                if (Contains(SlotRow(i), tap.Value))
                    return new FarmScreen(slots[i]);
            }

            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch, Area, "Carregar jogo");

            if (slots.Count == 0)
            {
                DrawRow(spriteBatch, TitleRow, "Nenhum save encontrado.", false);
                if (font != null)
                {
                    spriteBatch.DrawString(font,
                        "Use Novo Jogo na tela inicial para comecar.",
                        new Vector2(292, 268), Color.White * 0.8f);
                }
                DrawBack(spriteBatch);
                return;
            }

            for (var i = 0; i < slots.Count && i < MaxSlots; i++)
            {
                var slot = slots[i];
                var label = (i + 1) + ".  " + slot.FarmerName + "  -  " + slot.FarmName;
                if (!string.IsNullOrEmpty(slot.Created))
                    label += "  (" + slot.Created + ")";

                DrawRow(spriteBatch, SlotRow(i), label, false);
            }

            DrawBack(spriteBatch);
        }
    }
}
