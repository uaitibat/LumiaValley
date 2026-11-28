using System;
using System.Collections.Generic;
using Galaxy.Api;
using Steamworks;

namespace StardewValley.SDKs;

internal class SteamNetHelper : GalaxyNetHelper
{
	private Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;

	private Callback<LobbyEnter_t> lobbyEnterCallback;

	private Callback<LobbyDataUpdate_t> lobbyDataUpdateCallback;

	private Dictionary<ulong, CSteamID> lobbyOwners = new Dictionary<ulong, CSteamID>();

	public SteamNetHelper()
	{
		gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(onGameLobbyJoinRequested);
		lobbyEnterCallback = Callback<LobbyEnter_t>.Create(onLobbyEnter);
		lobbyDataUpdateCallback = Callback<LobbyDataUpdate_t>.Create(onLobbyDataUpdate);
	}

	public override void RequestFriendLobbyData()
	{
		EFriendFlags iFriendFlags = EFriendFlags.k_EFriendFlagImmediate;
		int friendCount = SteamFriends.GetFriendCount(iFriendFlags);
		for (int i = 0; i < friendCount; i++)
		{
			if (SteamFriends.GetFriendGamePlayed(SteamFriends.GetFriendByIndex(i, iFriendFlags), out var pFriendGameInfo) && !(pFriendGameInfo.m_gameID.AppID() != SteamUtils.GetAppID()))
			{
				SteamMatchmaking.RequestLobbyData(pFriendGameInfo.m_steamIDLobby);
			}
		}
	}

	public override void ShowInviteDialog(object lobby)
	{
		SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID((ulong)lobby));
	}

	private void onLobbyDataUpdate(LobbyDataUpdate_t pCallback)
	{
		CSteamID steamIDLobby = new CSteamID(pCallback.m_ulSteamIDLobby);
		string lobbyData = SteamMatchmaking.GetLobbyData(steamIDLobby, "connect");
		GalaxyID galaxyID = parseConnectionString(lobbyData);
		if (!(galaxyID == null))
		{
			lobbyOwners[galaxyID.ToUint64()] = SteamMatchmaking.GetLobbyOwner(steamIDLobby);
			GalaxyInstance.Matchmaking().RequestLobbyData(galaxyID);
		}
	}

	protected override GalaxyID getStartupLobby()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i] == "+connect_lobby")
			{
				try
				{
					SteamMatchmaking.JoinLobby(new CSteamID(Convert.ToUInt64(commandLineArgs[i + 1])));
					return null;
				}
				catch (Exception)
				{
				}
			}
		}
		return null;
	}

	private void onGameLobbyJoinRequested(GameLobbyJoinRequested_t pCallback)
	{
		SteamMatchmaking.JoinLobby(pCallback.m_steamIDLobby);
	}

	private void onLobbyEnter(LobbyEnter_t pCallback)
	{
		CSteamID steamIDLobby = new CSteamID(pCallback.m_ulSteamIDLobby);
		if (!(SteamMatchmaking.GetLobbyOwner(steamIDLobby) == SteamUser.GetSteamID()))
		{
			lobbyRequested = parseConnectionString(SteamMatchmaking.GetLobbyData(steamIDLobby, "connect"));
			SteamMatchmaking.LeaveLobby(steamIDLobby);
			if (lobbyRequested != null)
			{
				Game1.multiplayer.inviteAccepted();
			}
		}
	}

	public override string GetLobbyOwnerName(object lobbyID)
	{
		if (lobbyOwners.ContainsKey((ulong)lobbyID))
		{
			return SteamFriends.GetFriendPersonaName(lobbyOwners[(ulong)lobbyID]);
		}
		return "";
	}
}
