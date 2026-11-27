// Substitutos dos SDKs externos (Steam/GOG/Lidgren) removidos do build UWP.
// Assinaturas copiadas de FullSrc/StardewValley.SDKs e de Client/IGameServer.
using System;
using StardewValley.Network;

namespace StardewValley.SDKs
{
    public interface LobbyUpdateListener { void OnLobbyUpdate(object lobby); }

    public interface SDKNetHelper
    {
        string GetUserID();
        Client CreateClient(object lobby);
        Client GetRequestedClient();
        Server CreateServer(IGameServer gameServer);
        void AddLobbyUpdateListener(LobbyUpdateListener listener);
        void RemoveLobbyUpdateListener(LobbyUpdateListener listener);
        void RequestFriendLobbyData();
        string GetLobbyData(object lobby, string key);
        string GetLobbyOwnerName(object lobby);
        bool SupportsInviteCodes();
        object GetLobbyFromInviteCode(string inviteCode);
        void ShowInviteDialog(object lobby);
        void MutePlayer(string userId, bool mute);
        bool IsPlayerMuted(string userId);
        void ShowProfile(string userId);
    }

    public interface SDKHelper
    {
        bool IsEnterButtonAssignmentFlipped { get; }
        bool IsJapaneseRegionRelease { get; }
        string Name { get; }
        SDKNetHelper Networking { get; }
        bool ConnectionFinished { get; }
        int ConnectionProgress { get; }
        bool HasOverlay { get; }
        void EarlyInitialize();
        void Initialize();
        void Update();
        void Shutdown();
        void DebugInfo();
        void GetAchievement(string achieve);
        void ResetAchievements();
        string FilterDirtyWords(string words);
    }

    public class NullSDKNetHelper : SDKNetHelper
    {
        public string GetUserID() { return ""; }
        public Client CreateClient(object lobby) { return null; }
        public Client GetRequestedClient() { return null; }
        public Server CreateServer(IGameServer gameServer) { return null; }
        public void AddLobbyUpdateListener(LobbyUpdateListener listener) { }
        public void RemoveLobbyUpdateListener(LobbyUpdateListener listener) { }
        public void RequestFriendLobbyData() { }
        public string GetLobbyData(object lobby, string key) { return ""; }
        public string GetLobbyOwnerName(object lobby) { return ""; }
        public bool SupportsInviteCodes() { return false; }
        public object GetLobbyFromInviteCode(string inviteCode) { return null; }
        public void ShowInviteDialog(object lobby) { }
        public void MutePlayer(string userId, bool mute) { }
        public bool IsPlayerMuted(string userId) { return false; }
        public void ShowProfile(string userId) { }
    }

    public class NullSDKHelper : SDKHelper
    {
        public virtual bool IsEnterButtonAssignmentFlipped { get { return false; } }
        public virtual bool IsJapaneseRegionRelease { get { return false; } }
        public virtual string Name { get { return "Null"; } }
        public virtual SDKNetHelper Networking { get { return _networking; } }
        private readonly SDKNetHelper _networking = new NullSDKNetHelper();
        public virtual bool ConnectionFinished { get { return true; } }
        public virtual int ConnectionProgress { get { return 100; } }
        public virtual bool HasOverlay { get { return false; } }
        public virtual void EarlyInitialize() { }
        public virtual void Initialize() { }
        public virtual void Update() { }
        public virtual void Shutdown() { }
        public virtual void DebugInfo() { }
        public virtual void GetAchievement(string achieve) { }
        public virtual void ResetAchievements() { }
        public virtual string FilterDirtyWords(string words) { return words; }
    }

    public sealed class SteamHelper : NullSDKHelper
    {
        public bool active { get { return false; } }
        public void CancelKeyboard() { }
        public bool IsRunningOnSteamDeck() { return false; }
    }
}

namespace StardewValley
{
    public sealed class LidgrenClient : Client
    {
        private readonly string _address;
        public LidgrenClient(string address) { _address = address; }
        public LidgrenClient(string address, int port) { _address = address; }
        protected override void connectImpl() { }
        public override void disconnect(bool neatly = true) { }
        protected override void receiveMessagesImpl() { }
        public override void sendMessage(OutgoingMessage message) { }
        public override string getUserID() { return ""; }
        protected override string getHostUserName() { return ""; }
    }

    public sealed class GameServer : IGameServer
    {
        public GameServer() { }
        public GameServer(bool local_multiplayer) { }
        public BandwidthLogger BandwidthLogger { get { return null; } }
        public bool LogBandwidth { get; set; }
        public int connectionsCount { get { return 0; } }
        public string getInviteCode() { return ""; }
        public string getUserName(long farmerId) { return ""; }
        public void setPrivacy(ServerPrivacy privacy) { }
        public void stopServer() { }
        public void receiveMessages() { }
        public void sendMessage(long peerId, OutgoingMessage message) { }
        public bool canAcceptIPConnections() { return false; }
        public bool canOfferInvite() { return false; }
        public void offerInvite() { }
        public bool connected() { return false; }
        public void sendMessage(long peerId, byte messageType, Farmer sourceFarmer, params object[] data) { }
        public void sendMessages() { }
        public void startServer() { }
        public void initializeHost() { }
        public void sendServerIntroduction(long peer) { }
        public void kick(long disconnectee) { }
        public string ban(long farmerId) { return ""; }
        public void playerDisconnected(long disconnectee) { }
        public bool isGameAvailable() { return false; }
        public bool whenGameAvailable(Action action, Func<bool> customAvailabilityCheck = null) { return false; }
        public void checkFarmhandRequest(string userID, string connectionID, NetFarmerRoot farmer, Action<OutgoingMessage> sendMessage, Action approve) { }
        public void sendAvailableFarmhands(string userID, Action<OutgoingMessage> sendMessage) { }
        public void processIncomingMessage(IncomingMessage message) { }
        public void updateLobbyData() { }
        public float getPingToClient(long peer) { return 0f; }
        public bool isUserBanned(string userID) { return false; }
        public void onConnect(string connectionID) { }
        public void onDisconnect(string connectionID) { }
        public bool IsLocalMultiplayerInitiatedServer() { return false; }
    }
}
