// SteamWrapper.h

#pragma once

#include "steam_api.h"

using namespace System;

namespace SteamWrapper
{

	public ref class SteamCore
	{

	public:

		static bool RestartViaSteamIfNecessary( uint32 AppId );

		static bool Initialize();
		static void Shutdown();
		static void Update();

		static String^ PlayerName();
	};

	public ref class SteamTextInput
	{

	internal:

		static Action< bool >^ s_OnGamepadInputEnd;

	public:

		static const char * GetText();
		static bool ShowGamepadTextInput( System::String^ Description, System::String^ InitialText, unsigned int MaxCharacters, Action< bool >^ OnGamepadInputEnd );

	};

	public value class SteamUser
	{
	
	public:
		CSteamID* m_handle;

		SteamUser( CSteamID* handle ) :
			m_handle( handle )
		{
		}
	};

	public value class SteamLobby
	{

	public:
		CSteamID * m_handle;

		SteamLobby( CSteamID * handle ) :
			m_handle( handle )
		{
		}

		SteamLobby( uint64 handle ) :
			SteamLobby( new CSteamID( handle ) )
		{
		}
	};

	public value class LeaderboardHandle
	{

	public:
		SteamLeaderboard_t m_handle;

		LeaderboardHandle( SteamLeaderboard_t handle ) :
		   m_handle( handle )
		{
		}

	};

	class CallbackClass
	{

	public:

		CallbackClass();

		void OnFindLeaderboard( LeaderboardFindResult_t * pResult, bool bIOFailure );
		void OnLeaderboardDownloadedEntries( LeaderboardScoresDownloaded_t * pLeaderboardScoresDownloaded, bool bIOFailure );
		
		STEAM_CALLBACK( CallbackClass, OnGamepadInputEnd, GamepadTextInputDismissed_t, m_GamepadInputEnded );

		void OnFindLobbies( LobbyMatchList_t * pLobbyMatchList, bool bIOFailure );
		void OnJoinLobby( LobbyEnter_t * pCallback, bool bIOFailure );
		void OnLobbyCreated( LobbyCreated_t * pCallback, bool bIOFailure );
		void OnChatUpdate( LobbyChatUpdate_t * pCallback, bool bIOFailure );
		void OnDataUpdate( LobbyDataUpdate_t * pCallback, bool bIOFailure );

		STEAM_CALLBACK( CallbackClass, OnChatMsg, LobbyChatMsg_t, m_OnChatMsg );
		//STEAM_CALLBACK( CallbackClass, OnChatMsg, LobbyChatMsg_t, m_OnChatMsg );
		//STEAM_CALLBACK( CallbackClass, OnChatMsg, LobbyChatMsg_t, m_OnChatMsg );
	};

	// Global items. Not exposed to the C# wrapper.
	CallbackClass* g_CallbackClassInstance;
	CCallResult< CallbackClass, LeaderboardFindResult_t > g_CallResultFindLeaderboard;
	CCallResult< CallbackClass, LeaderboardScoresDownloaded_t > g_CallResultDownloadEntries;
	CCallResult< CallbackClass, LobbyMatchList_t > g_CallResultLobbyMatchList;
	CCallResult< CallbackClass, LobbyEnter_t > g_CallResultJoinLobby;
	CCallResult< CallbackClass, LobbyCreated_t > g_CallResultLobbyCreated;
	CCallResult< CallbackClass, LobbyChatUpdate_t > g_CallResultChatUpdate;
	CCallResult< CallbackClass, LobbyChatMsg_t > g_CallResultChatMsg;
	CCallResult< CallbackClass, LobbyDataUpdate_t > g_CallResultDataUpdate;
	const int nMaxLeaderboardEntries = 1000;
	LeaderboardEntry_t m_leaderboardEntries[nMaxLeaderboardEntries];

	public ref class SteamStats
	{

	internal:

		static Action< LeaderboardHandle, bool >^ s_OnFind;
		static Action< bool >^ s_OnDownload;

		static int s_nLeaderboardEntriesFound = 0;

	public:

		static bool Initialize();

		static bool GiveAchievement( String^ AchievementApiName );

		static int NumEntries( LeaderboardHandle Handle );
		static int NumEntriesFound();

		static void FindLeaderboard( String^ LeaderboardName, Action< LeaderboardHandle, bool >^ OnFind );

		static void UploadScore( LeaderboardHandle Handle, int Value );

		static void RequestEntries( LeaderboardHandle Handle, int RequestType, int Start, int End, Action< bool >^ OnDownload );

		static int Results_GetScore( int Index );
		static int Results_GetRank( int Index );
		static const char * Results_GetPlayer( int Index );
		static const int Results_GetId( int Index );

	};

	public ref class SteamMatches
	{

	internal:
		static Action< bool >^ s_OnFindLobbies;
		static Action< bool >^ s_OnJoinLobby;
		static Action< bool >^ s_OnCreateLobby;
		static Action< bool >^ s_OnChatUpdate;
		static Action< String^ >^ s_OnChatMsg;
		static Action< bool >^ s_OnDataUpdate;

		static int s_nLobbiesFound = 0;
		static SteamLobby s_CurrentLobby;

	public:
		static void FindLobbies(Action< bool >^ OnFind);
		
		static int const NumLobbies();
		static String^ GetLobbyData( int Index, String^ Key );
		static void JoinLobby( int Index,
			Action< bool >^ OnJoinLobby,
			Action< bool >^ OnChatUpdate,
			Action< String^ >^ OnChatMsg,
			Action< bool >^ OnDataUpdate );

		static void CreateLobby(Action< bool >^ OnCreateLobby);

		static void SetLobbyData( String^ Key, String^ Value );
		static String^ GetLobbyData( String^ Key );

		static void SendChatMsg( String^ Msg );
	};
}
