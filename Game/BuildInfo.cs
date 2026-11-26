namespace LumiaValley.Game
{
    /// <summary>
    /// Hash curto do commit atual, substituido pelo workflow de build antes
    /// de compilar. Serve para identificar a versao instalada no aparelho.
    /// </summary>
    public static class BuildInfo
    {
        public const string Hash = "@BUILD_SHA@";
    }
}
