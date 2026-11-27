// Stubs completos para o experimento de bulk port (SDKs removidos).
using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework
{
    public static class GameWindowExtensions
    {
        public static int GetDisplayIndex(this GameWindow w) { return 0; }
        public static Microsoft.Xna.Framework.Rectangle GetDisplayBounds(this GameWindow w) { return new Microsoft.Xna.Framework.Rectangle(0, 0, 1280, 720); }
        public static void CenterOnDisplay(this GameWindow w, object d) { }
    }
}

namespace Microsoft.Xna.Framework.Audio
{
    public class Cue
    {
        public string Name = "";
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsPaused { get; set; }
        public bool IsStopped { get { return !IsPlaying; } }
        public bool IsStopping { get; set; }
        public bool IsPitchBeingControlledByRPC { get; set; }
        public void Play() { IsPlaying = true; }
        public void Stop() { IsPlaying = false; }
        public void Pause() { IsPaused = true; }
        public void Resume() { IsPaused = false; }
        public void SetVariable(string n, float v) { }
        public float GetVariable(string n) { return 0f; }
        public void Dispose() { }
    }
    public class AudioEngine
    {
        public AudioEngine(string s) { }
        public Cue GetCue(string n) { return new Cue(); }
        public void Update() { }
        public int GetCategory(string n) { return 0; }
        public int GetCategoryIndex(string n) { return 0; }
        public object GetReverbSettings(string n) { return null; }
        public bool IsDisposed { get; set; }
        public void Dispose() { }
    }
    public class SoundBank
    {
        public SoundBank(AudioEngine e, string s) { }
        public Cue GetCue(string n) { return new Cue(); }
        public void AddCue(string n, Cue c) { }
        public CueDefinition GetCueDefinition(string n) { return new CueDefinition(); }
        public bool IsDisposed { get; set; }
        public bool IsInUse { get; set; }
        public void PlayCue(string n) { }
        public void Dispose() { }
    }
    public class WaveBank { public WaveBank(AudioEngine e, string s) { } }
    public class CueDefinition { public string Name; }
}

namespace Lidgren.Network
{
    public enum NetConnectionStatus { Disconnected, Connecting, Connected, Disconnecting }
    public class NetIncomingMessage
    {
        public object SenderConnection;
        public object SenderEndPoint;
        public NetIncomingMessageType MessageType;
        public int LengthBits { get; set; }
        public int LengthBytes { get; set; }
        public int Position { get; set; }
        public string ReadString() { return ""; }
        public byte ReadByte() { return 0; }
        public int ReadInt32() { return 0; }
        public long ReadInt64() { return 0; }
        public float ReadFloat() { return 0f; }
        public bool ReadBoolean() { return false; }
        public void ReadBytes(byte[] b, int n) { }
    }
    public enum NetIncomingMessageType { Data, ConnectionApproval, StatusChanged, DiscoveryRequest, ErrorMessage }
    public class NetConnection
    {
        public object Tag;
        public object RemoteEndPoint;
        public object RemoteUniqueIdentifier;
        public NetConnectionStatus Status { get; set; }
        public double AverageRoundtripTime { get; set; }
        public void SendMessage(object m, bool r) { }
        public void Disconnect(string s) { }
        public void Approve() { }
        public void Deny() { }
    }
    public class NetBuffer
    {
        public NetBuffer() { }
        public NetBuffer(byte[] b) { }
        public int LengthBits { get; set; }
        public int Position { get; set; }
        public void Write(string s) { }
        public void Write(byte b) { }
        public void Write(int i) { }
        public void Write(long l) { }
        public void Write(bool b) { }
        public byte[] ReadBytes(int n) { return new byte[n]; }
        public string ReadString() { return ""; }
    }
    public class NetPeerConfiguration
    {
        public NetPeerConfiguration(string s) { }
        public int Port;
        public int MaximumConnections;
        public int ConnectionTimeout;
        public int MaximumTransmissionUnit;
        public float PingInterval;
        public void EnableMessageType(NetIncomingMessageType t) { }
    }
    public class NetClient
    {
        public NetClient(NetPeerConfiguration c) { }
        public NetIncomingMessage ReadMessage() { return null; }
        public void Recycle(NetIncomingMessage m) { }
        public void Start() { }
        public NetConnection Connect(string h, int p) { return new NetConnection(); }
        public NetBuffer CreateMessage() { return new NetBuffer(); }
        public void SendMessage(NetBuffer m, NetDeliveryMethod d) { }
        public void Disconnect(string s) { }
        public void FlushSendQueue() { }
        public NetConnectionStatus ConnectionStatus { get; set; }
        public object ServerConnection { get; set; }
        public long UniqueIdentifier { get; set; }
        public void DiscoverKnownPeer(string h, int p) { }
    }
    public class NetServer
    {
        public NetServer(NetPeerConfiguration c) { }
        public NetIncomingMessage ReadMessage() { return null; }
        public void Recycle(NetIncomingMessage m) { }
        public void Start() { }
        public void Shutdown(string s) { }
        public NetBuffer CreateMessage() { return new NetBuffer(); }
        public void SendMessage(NetBuffer m, NetDeliveryMethod d) { }
        public void SendDiscoveryResponse(NetBuffer m, object e) { }
        public void FlushSendQueue() { }
        public List<NetConnection> Connections = new List<NetConnection>();
        public int ConnectionsCount { get { return Connections.Count; } }
    }
    public enum NetDeliveryMethod { Unreliable, ReliableOrdered }
}

namespace SkiaSharp
{
    public enum SKColorType { Rgba8888, Rgb888x, Bgra8888 }
    public enum SKAlphaType { Premul, Opaque }
    public enum SKEncodedImageFormat { Png, Jpeg }
    public struct SKRect { public float X, Y, Width, Height; public static SKRect Create(float x, float y, float w, float h) { return new SKRect { X = x, Y = y, Width = w, Height = h }; } }
    public class SKPaint { }
    public class SKCanvas { public void DrawBitmap(SKBitmap b, SKRect r, SKPaint p) { } }
    public class SKSurface
    {
        public SKCanvas Canvas = new SKCanvas();
        public static SKSurface Create(object a, int w, int h, SKColorType c, SKAlphaType al) { return new SKSurface(); }
        public SKImage Snapshot() { return new SKImage(); }
        public void Dispose() { }
    }
    public class SKImage { public SKData Encode(SKEncodedImageFormat f, int q) { return new SKData(); } }
    public class SKData { public void SaveTo(System.IO.Stream s) { } }
    public class SKBitmap
    {
        public SKBitmap(int w, int h, SKColorType c, SKAlphaType a) { }
        public IntPtr GetPixels() { return IntPtr.Zero; }
        public void Dispose() { }
    }
}

namespace Steamworks
{
    public struct CSteamID { public ulong Value; public ulong m_ulSteamIDLobby; public CSteamID(ulong v) { Value = v; m_ulSteamIDLobby = 0; } }
    public struct GameOverlayActivated_t { public byte m_eResult; public byte Active; }
    public struct GameLobbyJoinRequested_t { public CSteamID SteamIDLobby; }
    public struct EncryptedAppTicketResponse_t { public object m_eResult; }
    public struct GamepadTextInputDismissed_t { public bool Submitted; }
    public enum EGamepadTextInputMode { Normal = 0, Password = 1 }
    public enum EGamepadTextInputLineMode { SingleLine = 0, MultipleLines = 1 }
    public enum EFriendFlags { None = 0, Friend = 1 }
    public class Callback<T> : IDisposable { public Callback(Action<T> a) { } public void Dispose() { } }
    public class CallResult<T> : IDisposable
    {
        public CallResult(Action<T> a) { }
        public void Set(object h) { }
        public void Dispose() { }
    }
    public static class SteamAPI { public static bool Init() { return false; } public static void Shutdown() { } public static void RunCallbacks() { } }
    public static class SteamUser { public static CSteamID GetSteamID() { return new CSteamID(0); } }
    public static class SteamFriends { public static string GetPersonaName() { return ""; } }
    public static class SteamUtils { public static int GetAppID() { return 0; } }
    public static class SteamUserStats { public static bool RequestCurrentStats() { return false; } public static bool GetAchievement(string n, out bool a) { a = false; return false; } public static bool ResetAchievements() { return false; } }
    public static class SteamMatchmaking { public static CSteamID CreateLobby(object t, int max) { return new CSteamID(0); } public static void JoinLobby(CSteamID id) { } public static string GetLobbyData(CSteamID id, string k) { return ""; } }
}

namespace TextCopy
{
    public static class Clipboard { public static string GetText() { return ""; } public static void SetText(string s) { } }
    public static class ClipboardService { public static string GetText() { return ""; } public static void SetText(string s) { } }
}

namespace StardewValley.SDKs
{
    public interface LobbyUpdateListener { void OnLobbyUpdate(object lobby); }
    public abstract class SDKHelper
    {
        public abstract class NetworkHelper
        {
            public virtual void AddLobbyUpdateListener(LobbyUpdateListener l) { }
            public virtual void RemoveLobbyUpdateListener(LobbyUpdateListener l) { }
            public virtual void UpdateNetwork() { }
            public virtual object CreateClient() { return null; }
            public virtual object CreateServer() { return null; }
            public virtual string GetLobbyData(object lobby, string key) { return ""; }
            public virtual object GetLobbyFromInviteCode(string code) { return null; }
            public virtual string GetLobbyOwnerName(object lobby) { return ""; }
            public virtual object GetRequestedClient() { return null; }
            public virtual string GetUserID() { return ""; }
            public virtual void RequestFriendLobbyData(object lobby) { }
            public virtual bool SupportsInviteCodes() { return false; }
            public virtual void Dispose() { }
        }
        public NetworkHelper Networking = new DummyNetworkHelper();
        public virtual void EarlyInitialize() { }
        public virtual void Update() { }
        public virtual void Initialize() { }
        public virtual void Shutdown() { }
        public virtual string DebugInfo() { return ""; }
        public virtual string ConnectionProgress() { return ""; }
        public virtual bool ConnectionFinished() { return true; }
        public virtual bool HasOverlay() { return false; }
        public virtual bool IsJapaneseRegionRelease() { return false; }
        public virtual bool IsEnterButtonAssignmentFlipped { get { return false; } }
        public virtual string FilterDirtyWords(string s) { return s; }
        public virtual bool GetAchievement(string n, out bool a) { a = false; return false; }
        public virtual void ResetAchievements() { }
        public virtual string GetFriendInviteArgs() { return ""; }
    }
    public sealed class DummyNetworkHelper : SDKHelper.NetworkHelper { }
}
