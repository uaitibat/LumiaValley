using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Galaxy.Api;
using StardewValley.Network;
using Steamworks;

namespace StardewValley.SDKs;

public class GalaxySocket
{
	private class GalaxyLobbyCreatedListener : ILobbyCreatedListener
	{
		private Action<GalaxyID, LobbyCreateResult> callback;

		public GalaxyLobbyCreatedListener(Action<GalaxyID, LobbyCreateResult> callback)
		{
			this.callback = callback;
			GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyCreated.GetListenerType(), this);
		}

		public override void OnLobbyCreated(GalaxyID lobbyID, LobbyCreateResult result)
		{
			callback(lobbyID, result);
		}

		public override void Dispose()
		{
			GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyCreated.GetListenerType(), this);
			base.Dispose();
		}
	}

	private class GalaxyLobbyEnteredListener : ILobbyEnteredListener
	{
		private Action<GalaxyID, LobbyEnterResult> callback;

		public GalaxyLobbyEnteredListener(Action<GalaxyID, LobbyEnterResult> callback)
		{
			this.callback = callback;
			GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyEntered.GetListenerType(), this);
		}

		public override void OnLobbyEntered(GalaxyID lobbyID, LobbyEnterResult result)
		{
			callback(lobbyID, result);
		}

		public override void Dispose()
		{
			GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyEntered.GetListenerType(), this);
			base.Dispose();
		}
	}

	private class GalaxyLobbyLeftListener : ILobbyLeftListener
	{
		private Action<GalaxyID, LobbyLeaveReason> callback;

		public GalaxyLobbyLeftListener(Action<GalaxyID, LobbyLeaveReason> callback)
		{
			this.callback = callback;
			GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyLeft.GetListenerType(), this);
		}

		public override void OnLobbyLeft(GalaxyID lobbyID, LobbyLeaveReason leaveReason)
		{
			callback(lobbyID, leaveReason);
		}

		public override void Dispose()
		{
			GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyLeft.GetListenerType(), this);
			base.Dispose();
		}
	}

	public const long Timeout = 30000L;

	private const int SendMaxPacketSize = 1100;

	private const int ReceiveMaxPacketSize = 1300;

	private const long RecreateLobbyDelay = 20000L;

	private const long HeartbeatDelay = 15000L;

	private const byte HeartbeatMessage = byte.MaxValue;

	public bool isRecreatedLobby;

	public bool isFirstRecreateAttempt;

	private GalaxyID selfId;

	private GalaxyID connectingLobbyID;

	private GalaxyID lobby;

	private GalaxyID lobbyOwner;

	private GalaxyLobbyEnteredListener galaxyLobbyEnterCallback;

	private GalaxyLobbyCreatedListener galaxyLobbyCreatedCallback;

	private GalaxyLobbyLeftListener galaxyLobbyLeftCallback;

	private string protocolVersion;

	private Dictionary<string, string> lobbyData = new Dictionary<string, string>();

	private ServerPrivacy privacy;

	private uint memberLimit;

	private long recreateTimer;

	private long heartbeatTimer;

	private Dictionary<ulong, GalaxyID> connections = new Dictionary<ulong, GalaxyID>();

	private HashSet<ulong> ghosts = new HashSet<ulong>();

	private Dictionary<ulong, MemoryStream> incompletePackets = new Dictionary<ulong, MemoryStream>();

	private Dictionary<ulong, long> lastMessageTime = new Dictionary<ulong, long>();

	private CSteamID? steamLobby;

	private Callback<LobbyEnter_t> steamLobbyEnterCallback;

	public int ConnectionCount => connections.Count;

	public IEnumerable<GalaxyID> Connections => connections.Values;

	public bool Connected => lobby != null;

	public GalaxyID LobbyOwner => lobbyOwner;

	public GalaxyID Lobby => lobby;

	public ulong? InviteDialogLobby
	{
		get
		{
			if (!steamLobby.HasValue)
			{
				return null;
			}
			return steamLobby.Value.m_SteamID;
		}
	}

	public GalaxySocket(string protocolVersion)
	{
		this.protocolVersion = protocolVersion;
		lobbyData["protocolVersion"] = protocolVersion;
		selfId = GalaxyInstance.User().GetGalaxyID();
		galaxyLobbyEnterCallback = new GalaxyLobbyEnteredListener(onGalaxyLobbyEnter);
		galaxyLobbyCreatedCallback = new GalaxyLobbyCreatedListener(onGalaxyLobbyCreated);
	}

	public string GetInviteCode()
	{
		if (lobby == null)
		{
			return null;
		}
		return Base36.Encode(lobby.GetRealID());
	}

	private string getConnectionString()
	{
		if (lobby == null)
		{
			return "";
		}
		return "-connect-lobby-" + lobby.ToUint64();
	}

	private long getTimeNow()
	{
		return DateTime.Now.Ticks / 10000;
	}

	public long GetPingWith(GalaxyID peer)
	{
		long value = 0L;
		lastMessageTime.TryGetValue(peer.ToUint64(), out value);
		if (value == 0L)
		{
			return 0L;
		}
		if (getTimeNow() - value > 30000)
		{
			return long.MaxValue;
		}
		return GalaxyInstance.Networking().GetPingWith(peer);
	}

	private LobbyType privacyToLobbyType(ServerPrivacy privacy)
	{
		return privacy switch
		{
			ServerPrivacy.InviteOnly => LobbyType.LOBBY_TYPE_PRIVATE, 
			ServerPrivacy.FriendsOnly => LobbyType.LOBBY_TYPE_FRIENDS_ONLY, 
			ServerPrivacy.Public => LobbyType.LOBBY_TYPE_PUBLIC, 
			_ => throw new ArgumentException(Convert.ToString(privacy)), 
		};
	}

	private ELobbyType privacyToSteamLobbyType(ServerPrivacy privacy)
	{
		return privacy switch
		{
			ServerPrivacy.InviteOnly => ELobbyType.k_ELobbyTypePrivate, 
			ServerPrivacy.FriendsOnly => ELobbyType.k_ELobbyTypeFriendsOnly, 
			ServerPrivacy.Public => ELobbyType.k_ELobbyTypePublic, 
			_ => throw new ArgumentException(Convert.ToString(privacy)), 
		};
	}

	public void SetPrivacy(ServerPrivacy privacy)
	{
		this.privacy = privacy;
		updateLobbyPrivacy();
	}

	public void CreateLobby(ServerPrivacy privacy, uint memberLimit)
	{
		this.privacy = privacy;
		this.memberLimit = memberLimit;
		lobbyOwner = selfId;
		isRecreatedLobby = false;
		tryCreateLobby();
	}

	private void tryCreateLobby()
	{
		Console.WriteLine("Creating lobby...");
		if (galaxyLobbyLeftCallback != null)
		{
			galaxyLobbyLeftCallback.Dispose();
			galaxyLobbyLeftCallback = null;
		}
		galaxyLobbyLeftCallback = new GalaxyLobbyLeftListener(onGalaxyLobbyLeft);
		GalaxyInstance.Matchmaking().CreateLobby(privacyToLobbyType(privacy), memberLimit, joinable: true, LobbyTopologyType.LOBBY_TOPOLOGY_TYPE_STAR);
		recreateTimer = 0L;
	}

	public void JoinLobby(GalaxyID lobbyId, Action<string> onError)
	{
		try
		{
			connectingLobbyID = lobbyId;
			GalaxyInstance.Matchmaking().JoinLobby(connectingLobbyID);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			string text = Game1.content.LoadString("Strings\\UI:CoopMenu_Failed");
			text = ((!ex.Message.EndsWith("already joined this lobby")) ? (text + " (" + ex.Message + ")") : (text + " (already connected)"));
			onError(text);
			Close();
		}
	}

	public void SetLobbyData(string key, string value)
	{
		lobbyData[key] = value;
		if (lobby != null)
		{
			GalaxyInstance.Matchmaking().SetLobbyData(lobby, key, value);
		}
	}

	private void updateLobbyPrivacy()
	{
		if (lobbyOwner != selfId)
		{
			return;
		}
		if (lobby != null)
		{
			GalaxyInstance.Matchmaking().SetLobbyType(lobby, privacyToLobbyType(privacy));
		}
		if (lobby == null)
		{
			if (steamLobby.HasValue)
			{
				SteamMatchmaking.LeaveLobby(steamLobby.Value);
			}
		}
		else if (!steamLobby.HasValue)
		{
			if (steamLobbyEnterCallback == null)
			{
				steamLobbyEnterCallback = Callback<LobbyEnter_t>.Create(onSteamLobbyEnter);
			}
			SteamMatchmaking.CreateLobby(privacyToSteamLobbyType(privacy), (int)memberLimit);
		}
		else
		{
			SteamMatchmaking.SetLobbyType(steamLobby.Value, privacyToSteamLobbyType(privacy));
			SteamMatchmaking.SetLobbyData(steamLobby.Value, "connect", getConnectionString());
		}
	}

	private void onGalaxyLobbyCreated(GalaxyID lobbyID, LobbyCreateResult result)
	{
		if (result != LobbyCreateResult.LOBBY_CREATE_RESULT_ERROR)
		{
			return;
		}
		Console.WriteLine("Failed to create lobby.");
		if (Game1.chatBox != null && isFirstRecreateAttempt)
		{
			if (isRecreatedLobby)
			{
				Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyCreateFail"));
			}
			else
			{
				Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyCreateFail"));
			}
		}
		recreateTimer = getTimeNow() + 20000;
		isRecreatedLobby = true;
		isFirstRecreateAttempt = false;
	}

	private void onGalaxyLobbyLeft(GalaxyID lobbyID, ILobbyLeftListener.LobbyLeaveReason leaveReason)
	{
		if (leaveReason != ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT)
		{
			Program.WriteLog(Program.LogType.Disconnect, "Forcibly left Galaxy lobby at " + DateTime.Now.ToLongTimeString() + " - " + leaveReason, append: true);
		}
		if (Game1.chatBox != null)
		{
			string sub = "";
			switch (leaveReason)
			{
			case ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_CONNECTION_LOST:
				sub = Game1.content.LoadString("Strings\\UI:Chat_LobbyLost_ConnectionLost");
				break;
			case ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_LOBBY_CLOSED:
				sub = Game1.content.LoadString("Strings\\UI:Chat_LobbyLost_LobbyClosed");
				break;
			case ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT:
				sub = Game1.content.LoadString("Strings\\UI:Chat_LobbyLost_UserLeft");
				break;
			}
			Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyLost", sub).Trim());
		}
		Console.WriteLine("Left lobby {0} - leaveReason: {1}", lobbyID.ToUint64(), leaveReason);
		lobby = null;
		recreateTimer = getTimeNow() + 20000;
		isRecreatedLobby = true;
		isFirstRecreateAttempt = true;
	}

	private void onGalaxyLobbyEnter(GalaxyID lobbyID, LobbyEnterResult result)
	{
		connectingLobbyID = null;
		if (result != 0)
		{
			return;
		}
		Console.WriteLine("Lobby entered: {0}", lobbyID.ToUint64());
		lobby = lobbyID;
		lobbyOwner = GalaxyInstance.Matchmaking().GetLobbyOwner(lobbyID);
		if (Game1.chatBox != null)
		{
			string sub = "";
			if (Program.sdk.Networking != null && Program.sdk.Networking.SupportsInviteCodes())
			{
				sub = Game1.content.LoadString("Strings\\UI:Chat_LobbyJoined_InviteCode", GetInviteCode());
			}
			if (isRecreatedLobby)
			{
				Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyRecreated", sub).Trim());
			}
			else
			{
				Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyJoined", sub).Trim());
			}
		}
		if (!(lobbyOwner == selfId))
		{
			return;
		}
		foreach (KeyValuePair<string, string> lobbyDatum in lobbyData)
		{
			GalaxyInstance.Matchmaking().SetLobbyData(lobby, lobbyDatum.Key, lobbyDatum.Value);
		}
		updateLobbyPrivacy();
	}

	private void onSteamLobbyEnter(LobbyEnter_t pCallback)
	{
		if (pCallback.m_EChatRoomEnterResponse == 1)
		{
			Console.WriteLine("Steam lobby entered: {0}", pCallback.m_ulSteamIDLobby);
			steamLobbyEnterCallback.Unregister();
			steamLobbyEnterCallback = null;
			steamLobby = new CSteamID(pCallback.m_ulSteamIDLobby);
			if (SteamMatchmaking.GetLobbyOwner(steamLobby.Value) == SteamUser.GetSteamID())
			{
				SteamMatchmaking.SetLobbyType(steamLobby.Value, privacyToSteamLobbyType(privacy));
				SteamMatchmaking.SetLobbyData(steamLobby.Value, "connect", getConnectionString());
			}
		}
	}

	public IEnumerable<GalaxyID> LobbyMembers()
	{
		if (lobby == null)
		{
			yield break;
		}
		uint lobby_members_count;
		try
		{
			lobby_members_count = GalaxyInstance.Matchmaking().GetNumLobbyMembers(lobby);
		}
		catch (Exception)
		{
			yield break;
		}
		uint i = 0u;
		while (i < lobby_members_count)
		{
			GalaxyID lobbyMemberByIndex = GalaxyInstance.Matchmaking().GetLobbyMemberByIndex(lobby, i);
			if (!(lobbyMemberByIndex == selfId) && !ghosts.Contains(lobbyMemberByIndex.ToUint64()))
			{
				yield return lobbyMemberByIndex;
			}
			uint num = i + 1;
			i = num;
		}
	}

	private bool lobbyContains(GalaxyID user)
	{
		foreach (GalaxyID item in LobbyMembers())
		{
			if (user == item || ghosts.Contains(item.ToUint64()))
			{
				return true;
			}
		}
		return false;
	}

	private void close(GalaxyID peer)
	{
		connections.Remove(peer.ToUint64());
		incompletePackets.Remove(peer.ToUint64());
	}

	public void Kick(GalaxyID user)
	{
		ghosts.Add(user.ToUint64());
	}

	public void Close()
	{
		if (connectingLobbyID != null)
		{
			GalaxyInstance.Matchmaking().LeaveLobby(connectingLobbyID);
			connectingLobbyID = null;
		}
		if (lobby != null)
		{
			while (ConnectionCount > 0)
			{
				close(Connections.First());
			}
			GalaxyInstance.Matchmaking().LeaveLobby(lobby);
			lobby = null;
		}
		updateLobbyPrivacy();
		try
		{
			galaxyLobbyEnterCallback.Dispose();
		}
		catch (Exception)
		{
		}
		try
		{
			galaxyLobbyCreatedCallback.Dispose();
		}
		catch (Exception)
		{
		}
		if (galaxyLobbyLeftCallback != null)
		{
			galaxyLobbyLeftCallback.Dispose();
		}
	}

	public void Receive(Action<GalaxyID> onConnection, Action<GalaxyID, Stream> onMessage, Action<GalaxyID> onDisconnect, Action<string> onError)
	{
		long timeNow = getTimeNow();
		if (lobby == null)
		{
			if (lobbyOwner == selfId && recreateTimer > 0 && recreateTimer <= timeNow)
			{
				recreateTimer = 0L;
				tryCreateLobby();
			}
			DisconnectPeers(onDisconnect);
			return;
		}
		try
		{
			string text = GalaxyInstance.Matchmaking().GetLobbyData(lobby, "protocolVersion");
			if (text != "" && text != protocolVersion)
			{
				onError(Game1.content.LoadString("Strings\\UI:CoopMenu_FailedProtocolVersion"));
				Close();
				return;
			}
		}
		catch (Exception)
		{
		}
		foreach (GalaxyID item in LobbyMembers())
		{
			if (!connections.ContainsKey(item.ToUint64()) && !ghosts.Contains(item.ToUint64()))
			{
				connections.Add(item.ToUint64(), item);
				onConnection(item);
			}
		}
		ghosts.IntersectWith(from peer in LobbyMembers()
			select peer.ToUint64());
		byte[] array = new byte[1300];
		uint outMsgSize = 1300u;
		GalaxyID outGalaxyID = new GalaxyID();
		while (GalaxyInstance.Networking().ReadP2PPacket(array, (uint)array.Length, ref outMsgSize, ref outGalaxyID))
		{
			lastMessageTime[outGalaxyID.ToUint64()] = timeNow;
			if (!connections.ContainsKey(outGalaxyID.ToUint64()) || array[0] == byte.MaxValue)
			{
				continue;
			}
			bool flag = array[0] == 1;
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(array, 4, (int)(outMsgSize - 4));
			if (incompletePackets.ContainsKey(outGalaxyID.ToUint64()))
			{
				memoryStream.Position = 0L;
				memoryStream.CopyTo(incompletePackets[outGalaxyID.ToUint64()]);
				if (!flag)
				{
					memoryStream = incompletePackets[outGalaxyID.ToUint64()];
					incompletePackets.Remove(outGalaxyID.ToUint64());
					memoryStream.Position = 0L;
					onMessage(outGalaxyID, memoryStream);
				}
			}
			else if (flag)
			{
				memoryStream.Position = memoryStream.Length;
				incompletePackets[outGalaxyID.ToUint64()] = memoryStream;
			}
			else
			{
				memoryStream.Position = 0L;
				onMessage(outGalaxyID, memoryStream);
			}
		}
		DisconnectPeers(onDisconnect);
	}

	public virtual void DisconnectPeers(Action<GalaxyID> onDisconnect)
	{
		List<GalaxyID> list = new List<GalaxyID>();
		foreach (GalaxyID value in connections.Values)
		{
			if (lobby == null || !lobbyContains(value) || ghosts.Contains(value.ToUint64()))
			{
				list.Add(value);
			}
		}
		foreach (GalaxyID item in list)
		{
			onDisconnect(item);
			close(item);
		}
	}

	public void Heartbeat(IEnumerable<GalaxyID> peers)
	{
		long timeNow = getTimeNow();
		if (heartbeatTimer > timeNow)
		{
			return;
		}
		heartbeatTimer = timeNow + 15000;
		byte[] array = new byte[1] { 255 };
		foreach (GalaxyID peer in peers)
		{
			GalaxyInstance.Networking().SendP2PPacket(peer, array, (uint)array.Length, P2PSendType.P2P_SEND_RELIABLE);
		}
	}

	public void Send(GalaxyID peer, byte[] data)
	{
		if (!connections.ContainsKey(peer.ToUint64()))
		{
			return;
		}
		if (data.Length <= 1100)
		{
			byte[] array = new byte[data.Length + 4];
			data.CopyTo(array, 4);
			GalaxyInstance.Networking().SendP2PPacket(peer, array, (uint)array.Length, P2PSendType.P2P_SEND_RELIABLE);
			return;
		}
		int num = 1096;
		int num2 = 0;
		byte[] array2 = new byte[1100];
		array2[0] = 1;
		while (num2 < data.Length)
		{
			int num3 = num;
			if (num2 + num >= data.Length)
			{
				array2[0] = 0;
				num3 = data.Length - num2;
			}
			Buffer.BlockCopy(data, num2, array2, 4, num3);
			num2 += num3;
			GalaxyInstance.Networking().SendP2PPacket(peer, array2, (uint)(num3 + 4), P2PSendType.P2P_SEND_RELIABLE);
		}
	}

	public void Send(GalaxyID peer, OutgoingMessage message)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter writer = new BinaryWriter(memoryStream);
		message.Write(writer);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		Send(peer, memoryStream.ToArray());
	}
}
