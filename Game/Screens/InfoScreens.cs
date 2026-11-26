using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.UI.Xaml;

namespace LumiaValley.Game.Screens
{
    /// <summary>Resposta honesta para Co-op: o port nao tem rede.</summary>
    public sealed class CoopScreen : MenuScreen
    {
        private static readonly Rectangle Area = new Rectangle(300, 160, 680, 400);

        public override IScreen Update(GameTime gameTime, Vector2? tap)
        {
            return BackTapped(tap) ? Home() : this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch, Area, "Co-op");
            DrawShadowText(spriteBatch, "Modo cooperativo indisponivel neste port.", font,
                new Vector2(340, 272), TextColor, 1.2f);
            DrawShadowText(spriteBatch, "Nao ha conexao de rede no escopo do LumiaValley.", font,
                new Vector2(340, 320), TextColor, 1.2f);
            DrawShadowText(spriteBatch, "Use Novo Jogo para comecar uma fazenda local.", font,
                new Vector2(340, 368), TextColor, 1.2f);
            DrawBack(spriteBatch);
        }
    }

    /// <summary>Confirmacao antes de fechar o app.</summary>
    public sealed class ConfirmExitScreen : MenuScreen
    {
        private static readonly Rectangle Area = new Rectangle(380, 190, 520, 340);
        private static readonly Rectangle YesRow = new Rectangle(430, 300, 420, 60);

        public override IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (BackTapped(tap))
                return Home();

            if (tap.HasValue && Contains(YesRow, tap.Value))
            {
                try
                {
                    Application.Current.Exit();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Nao foi possivel sair: " + ex.Message);
                }
            }

            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch, Area, "Sair do jogo?");
            DrawRow(spriteBatch, YesRow, "Sim, sair", true);
            DrawBack(spriteBatch);
        }
    }

    /// <summary>Sobre o port e a versao de origem dos dados.</summary>
    public sealed class AboutScreen : MenuScreen
    {
        private static readonly Rectangle Area = new Rectangle(280, 128, 720, 472);

        public override IScreen Update(GameTime gameTime, Vector2? tap)
        {
            return BackTapped(tap) ? Home() : this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch, Area, "Sobre");
            DrawShadowText(spriteBatch, "LumiaValley 0.1 - port nao oficial", font,
                new Vector2(320, 240), TextColor, 1.25f);
            DrawShadowText(spriteBatch, "Base: Stardew Valley 1.5.6 (Windows)", font,
                new Vector2(320, 296), TextColor, 1.25f);
            DrawShadowText(spriteBatch, "Feito para Lumia 640 / Windows 10 Mobile.", font,
                new Vector2(320, 352), TextColor, 1.25f);
            DrawShadowText(spriteBatch, "Saves proprios, sem compatibilidade com o PC.", font,
                new Vector2(320, 408), TextColor, 1.25f);
            DrawBack(spriteBatch);
        }
    }

    /// <summary>Opcoes basicas: som e resolucao da cena.</summary>
    public sealed class OptionsScreen : MenuScreen
    {
        private static readonly Rectangle Area = new Rectangle(280, 112, 720, 496);
        private static readonly Rectangle SoundRow = new Rectangle(330, 220, 620, 58);
        private static readonly Rectangle ScaleRow = new Rectangle(330, 300, 620, 58);
        private static readonly Rectangle LanguageRow = new Rectangle(330, 380, 620, 58);

        public override IScreen Update(GameTime gameTime, Vector2? tap)
        {
            if (BackTapped(tap))
                return Home();

            if (!tap.HasValue)
                return this;

            if (Contains(SoundRow, tap.Value))
                Settings.SoundEnabled = !Settings.SoundEnabled;
            else if (Contains(ScaleRow, tap.Value))
                Settings.HighQuality = !Settings.HighQuality;

            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch, Area, "Opcoes");
            DrawRow(spriteBatch, SoundRow,
                "Som:  " + (Settings.SoundEnabled ? "ligado" : "desligado"), true);
            DrawRow(spriteBatch, ScaleRow,
                "Cena:  " + (Settings.HighQuality ? "1280x720 (qualidade)" : "640x360 (leve)"), false);
            DrawRow(spriteBatch, LanguageRow, "Idioma:  Portugues do Brasil", false);
            DrawBack(spriteBatch);
        }
    }
}
