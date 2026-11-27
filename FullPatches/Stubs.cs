// Stubs minimos para SDKs incompativeis com UWP ARM (medicao de viabilidade).
using System;

namespace Galaxy.Api
{
    public struct GalaxyID { public ulong Value; public GalaxyID(ulong v) { Value = v; } public GalaxyID(string v) { Value = 0; } public override string ToString() { return Value.ToString(); } }
    public enum LobbyEnterResult { Success = 1, Failure = 0 }
    public enum LobbyCreateResult { Success = 1, Failure = 0 }
    public enum LobbyLeaveReason { Leaving = 1, Disconnect = 2 }
    public enum FailureReason { None = 0 }
    public struct LobbyEnter_t { public object Result; }
    public struct LobbyDataUpdate_t { public object LobbyID; }
    public class GalaxyApi { public static GalaxyApi Instance = new GalaxyApi(); public bool Init(object a) { return false; } public void Shutdown() { } public void AddListener(object l) { } }
}

namespace BmFont
{
    public class FontChar { public int X, Y, Width, Height, XOffset, YOffset, XAdvance; public char Char; }
    public class FontCommon { public int LineHeight; public int Base; public int ScaleW, ScaleH; }
    public class FontFile
    {
        public FontCommon Common = new FontCommon();
        public FontChar[] Chars = new FontChar[0];
        public static FontFile Load(System.IO.Stream s) { return new FontFile(); }
    }
}

