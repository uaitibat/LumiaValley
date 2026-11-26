namespace LumiaValley.Game
{
    /// <summary>
    /// Nomes e descricoes dos sete tipos de fazenda da 1.5.6. As descricoes
    /// vem dos XNB de UI quando disponiveis; o texto aqui e apenas fallback.
    /// </summary>
    public static class FarmTypes
    {
        public const int Count = 7;

        private static readonly string[] Names =
        {
            "Padrao", "Rio", "Floresta", "Morros", "Selvagem", "Quatro Cantos", "Praia"
        };

        private static readonly string[] Keys =
        {
            "Strings\\UI:Character_FarmStandard",
            "Strings\\UI:Character_FarmFishing",
            "Strings\\UI:Character_FarmForaging",
            "Strings\\UI:Character_FarmMining",
            "Strings\\UI:Character_FarmCombat",
            "Strings\\UI:Character_FarmFourCorners",
            "Strings\\UI:Character_FarmBeach"
        };

        private static readonly string[] Fallbacks =
        {
            "A fazenda classica: muito espaco para plantar, criar animais e pescar.",
            "Rios atravessam a terra. Otima para pesca, mas com menos espaco util.",
            "Arvores e tocos aparecem no comeco. Madeira e cogumelos sao comuns.",
            "O terreno tem elevacoes e pedras. A minacao rende mais aqui.",
            "Monstros aparecem a noite. Leve uma arma e feche as portas.",
            "Quatro areas separadas para ate quatro fazendeiros.",
            "Areia e ondas na beira da fazenda. Mariscos e peixes aparecem na praia."
        };

        public static int Clamp(int index)
        {
            if (index < 0)
                return 0;
            if (index >= Count)
                return Count - 1;
            return index;
        }

        public static string Name(int index)
        {
            return Names[Clamp(index)];
        }

        public static string Description(int index)
        {
            var i = Clamp(index);
            return Localization.UIString(Keys[i], Fallbacks[i]);
        }
    }
}
