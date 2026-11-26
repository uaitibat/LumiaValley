using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LumiaValley.Game.Screens
{
    /// <summary>
    /// Janela Avancado da criacao de personagem, equivalente ao
    /// AdvancedGameOptions da 1.5.6. As opcoes alteram o rascunho/Pettings
    /// locais do port; o botao OK devolve o jogador a criacao.
    /// </summary>
    public sealed class AdvancedOptionsScreen : MenuScreen
    {
        private static readonly Rectangle Area = new Rectangle(240, 110, 800, 500);
        private static readonly Rectangle OkRect = new Rectangle(240, 642, 64, 64);

        private static readonly string[] Rows =
        {
            "Dificuldade", "Cabins iniciais", "Layout das cabins", "Monstros na fazenda",
            "Ano 1 completavel", "Tesouros da mina", "Semente aleatoria"
        };

        private static readonly string[][] Values =
        {
            new[] { "Normal", "75%", "50%", "25%" },
            new[] { "Nenhum", "1", "2", "3" },
            new[] { "Proximo", "Separado" },
            new[] { "Nao", "Sim" },
            new[] { "Nao", "Sim" },
            new[] { "Normal", "Remixado" },
            new[] { "Desligada", "Ligada" }
        };

        // Valores padrao da 1.5.6.
        private readonly int[] indexes = { 0, 0, 0, 0, 0, 0, 0 };
        private readonly SaveSlot draft;

        public AdvancedOptionsScreen(SaveSlot draft)
        {
            this.draft = draft;
        }

        public override IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (!tap.HasValue)
                return this;

            if (Contains(OkRect, tap.Value))
                return new CharacterCreationScreen(draft);

            for (var i = 0; i < Rows.Length; i++)
            {
                if (!Contains(RowRect(i), tap.Value))
                    continue;
                indexes[i] = (indexes[i] + 1) % Values[i].Length;
                Apply(i);
                return this;
            }

            return this;
        }

        private Rectangle RowRect(int index)
        {
            return new Rectangle(Area.X + 24, Area.Y + 70 + index * 56, Area.Width - 48, 48);
        }

        private void Apply(int i)
        {
            switch (i)
            {
                case 3: draft.SpawnMonstersAtNight = indexes[i] == 1; break;
                case 4: draft.YearOneCompletable = indexes[i] == 1; break;
                case 6: draft.UseRandomSeed = indexes[i] == 1; break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch, Area, Localization.UIString("Strings\\UI:AGO_Label", "Opcoes avancadas"));

            for (var i = 0; i < Rows.Length; i++)
            {
                var rect = RowRect(i);
                DrawRow(spriteBatch, rect, Rows[i] + ":  " + Values[i][indexes[i]], false);
            }

            if (cursors == null)
            {
                DrawRow(spriteBatch, OkRect, "OK", true);
                return;
            }

            // Tile 46 com o tamanho padrao de 64px do mouseCursors.
            spriteBatch.Draw(cursors, OkRect, new Rectangle(128, 256, 64, 64), Color.White);
        }

        private Texture2D cursors;

        public override void Load(ContentManager content, GraphicsDevice device)
        {
            base.Load(content, device);
            try
            {
                cursors = content.Load<Texture2D>("LooseSprites/Cursors.pt-BR");
            }
            catch
            {
                try { cursors = content.Load<Texture2D>("LooseSprites/Cursors"); }
                catch { cursors = null; }
            }
        }
    }
}
