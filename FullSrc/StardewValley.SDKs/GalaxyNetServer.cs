using System;
using System.IO;
using Galaxy.Api;
using StardewValley.Network;

namespace StardewValley.SDKs;

public class GalaxyNetServer : Server
{
	private class GalaxyPersonaDataChangedListener : IPersonaDataChangedListener
	{
		private Action<GalaxyID, uint> callback;

		public GalaxyPersonaDataChangedListener(Action<GalaxyID, uint> callback)
		{
			this.callback = callback;
			GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerPersonaDataChanged.GetListenerType(), this);
		}

		public override void OnPersonaDataChanged(GalaxyID userID, uint avatarCriteria)
		{
			callback(userID, avatarCriteria);
		}

		public override void Dispose()
		{
			GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerPersonaDataChanged.GetListenerType(), this);
			base.Dispose();
		}
	}

	private GalaxyID host;

	protected GalaxySocket server;

	private GalaxyPersonaDataChangedListener galaxyPersonaDataChangedListener;

	protected Bimap<long, ulong> peers = new Bimap<long, ulong>();

	public override int connectionsCount
	{
		get
		{
			if (server == null)
			{
				return 0;
			}
			return server.ConnectionCount;
		}
	}

	public GalaxyNetServer(IGameServer gameServer)
		: base(gameServer)
	{
	}

	public override string getUserId(long farmerId)
	{
		if (!peers.ContainsLeft(farmerId))
		{
			return null;
		}
		return peers[farmerId].ToString();
	}

	public override bool hasUserId(string userId)
	{
		foreach (ulong rightValue in peers.RightValues)
		{
			if (rightValue.ToString().Equals(userId))
			{
				return true;
			}
		}
		return false;
	}

	public override bool isConnectionActive(string connection_id)
	{
		foreach (GalaxyID connection in server.Connections)
		{
			if (getConnectionId(connection) == connection_id && connection.IsValid())
			{
				return true;
			}
		}
		return false;
	}

	public override string getUserName(long farmerId)
	{
		if (!peers.ContainsLeft(farmerId))
		{
			return null;
		}
		GalaxyID userID = new GalaxyID(peers[farmerId]);
		return GalaxyInstance.Friends().GetFriendPersonaName(userID);
	}

	public override float getPingToClient(long farmerId)
	{
		if (!peers.ContainsLeft(farmerId))
		{
			return -1f;
		}
		GalaxyID peer = new GalaxyID(peers[farmerId]);
		return server.GetPingWith(peer);
	}

	public override void setPrivacy(ServerPrivacy privacy)
	{
		server.SetPrivacy(privacy);
	}

	public override bool connected()
	{
		return server.Connected;
	}

	public override bool canOfferInvite()
	{
		return server.Connected;
	}

	public override void offerInvite()
	{
		if (server.Connected && Program.sdk.Networking != null && server.InviteDialogLobby.HasValue)
		{
			Program.sdk.Networking.ShowInviteDialog(server.InviteDialogLobby.Value);
		}
	}

	public override string getInviteCode()
	{
		return server.GetInviteCode();
	}

	public override void initialize()
	{
		Console.WriteLine("Starting Galaxy server");
		host = GalaxyInstance.User().GetGalaxyID();
		galaxyPersonaDataChangedListener = new GalaxyPersonaDataChangedListener(onPersonaDataChanged);
		server = new GalaxySocket("1.5.5");
		server.CreateLobby(Game1.options.serverPrivacy, (uint)(Game1.multiplayer.playerLimit * 2));
	}

	public override void stopServer()
	{
		Console.WriteLine("Stopping Galaxy server");
		server.Close();
		if (galaxyPersonaDataChangedListener != null)
		{
			galaxyPersonaDataChangedListener.Dispose();
			galaxyPersonaDataChangedListener = null;
		}
	}

	private void onPersonaDataChanged(GalaxyID userID, uint avatarCriteria)
	{
		if (peers.ContainsRight(userID.ToUint64()))
		{
			long left = peers.GetLeft(userID.ToUint64());
			Game1.multiplayer.broadcastUserName(left, GalaxyInstance.Friends().GetFriendPersonaName(userID));
		}
	}

	public override void receiveMessages()
	{
		if (server == null)
		{
			return;
		}
		server.Receive(onReceiveConnection, onReceiveMessage, onReceiveDisconnect, onReceiveError);
		server.Heartbeat(server.LobbyMembers());
		foreach (GalaxyID connection in server.Connections)
		{
			if (server.GetPingWith(connection) > 30000)
			{
				server.Kick(connection);
			}
		}
		if (bandwidthLogger != null)
		{
			bandwidthLogger.Update();
		}
	}

	public override void kick(long disconnectee)
	{
		base.kick(disconnectee);
		if (peers.ContainsLeft(disconnectee))
		{
			GalaxyID galaxyID = new GalaxyID(peers[disconnectee]);
			server.Kick(galaxyID);
			Farmer player = Game1.player;
			object[] data = new Object[0];
			sendMessage(galaxyID, new OutgoingMessage(23, player, data));
		}
	}

	public string getConnectionId(GalaxyID peer)
	{
		return "GN_" + Convert.ToString(peer.ToUint64());
	}

	private string createUserID(GalaxyID peer)
	{
		return Convert.ToString(peer.ToUint64());
	}

	protected virtual void onReceiveConnection(GalaxyID peer)
	{
		if (!gameServer.isUserBanned(peer.ToString()))
		{
			Console.WriteLine("{0} connected", peer);
			onConnect(getConnectionId(peer));
			gameServer.sendAvailableFarmhands(createUserID(peer), delegate(OutgoingMessage msg)
			{
				sendMessage(peer, msg);
			});
		}
	}

	protected virtual void onReceiveMessage(GalaxyID peer, Stream messageStream)
	{
		if (bandwidthLogger != null)
		{
			bandwidthLogger.RecordBytesDown(messageStream.Length);
		}
		using IncomingMessage incomingMessage = new IncomingMessage();
		using BinaryReader reader = new BinaryReader(messageStream);
		incomingMessage.Read(reader);
		if (peers.ContainsLeft(incomingMessage.FarmerID) && peers[incomingMessage.FarmerID] == peer.ToUint64())
		{
			gameServer.processIncomingMessage(incomingMessage);
		}
		else if (incomingMessage.MessageType == 2)
		{
			NetFarmerRoot farmer = Game1.multiplayer.readFarmer(incomingMessage.Reader);
			GalaxyID capturedPeer = new GalaxyID(peer.ToUint64());
			gameServer.checkFarmhandRequest(createUserID(peer), getConnectionId(peer), farmer, delegate(OutgoingMessage msg)
			{
				sendMessage(capturedPeer, msg);
			}, delegate
			{
				peers[farmer.Value.UniqueMultiplayerID] = capturedPeer.ToUint64();
			});
		}
	}

	public virtual void onReceiveDisconnect(GalaxyID peer)
	{
		Console.WriteLine("{0} disconnected", peer);
		onDisconnect(getConnectionId(peer));
		if (peers.ContainsRight(peer.ToUint64()))
		{
			playerDisconnected(peers[peer.ToUint64()]);
		}
	}

	protected virtual void onReceiveError(string messageKey)
	{
		Console.WriteLine("Server error: " + Game1.content.LoadString(messageKey));
	}

	public override void playerDisconnected(long disconnectee)
	{
		base.playerDisconnected(disconnectee);
		peers.RemoveLeft(disconnectee);
	}

	public override void sendMessage(long peerId, OutgoingMessage message)
	{
		if (peers.ContainsLeft(peerId))
		{
			sendMessage(new GalaxyID(peers[peerId]), message);
		}
	}

	protected virtual void sendMessage(GalaxyID peer, OutgoingMessage message)
	{
		if (bandwidthLogger != null)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using BinaryWriter writer = new BinaryWriter(memoryStream);
				message.Write(writer);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				byte[] array = memoryStream.ToArray();
				server.Send(peer, array);
				bandwidthLogger.RecordBytesUp(array.Length);
				return;
			}
		}
		server.Send(peer, message);
	}

	public override void setLobbyData(string key, string value)
	{
		server.SetLobbyData(key, value);
	}
}
