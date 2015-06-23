// SteamWrapper.h

#pragma once

#include "steam_api.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SteamWrapper
{

	public ref class SteamCore
	{

	public:

		static bool RestartViaSteamIfNecessary( uint32 AppId );
		static bool SteamIsRunning();
		static bool SteamIsConnected();

		static void SetOfflineMode(bool Offline);
		static bool InOfflineMode();

		static bool Initialize();
		static void Shutdown();
		static void Update();

		static String^ PlayerName();
		static UInt64 PlayerId();

	private:

		static bool s_bOffline = false;

	};

	public ref class SteamTextInput
	{

	internal:

		static Action< bool >^ s_OnGamepadInputEnd;

	public:

		static const char * GetText();
		static bool ShowGamepadTextInput( System::String^ Description, System::String^ InitialText, unsigned int MaxCharacters, Action< bool >^ OnGamepadInputEnd );

	};

	public value class SteamLobby
	{

	public:
		CSteamID * m_handle = NULL;

		SteamLobby( CSteamID * handle ) :
			m_handle( handle )
		{
		}

		SteamLobby( uint64 handle ) :
			SteamLobby( new CSteamID( handle ) )
		{
		}
	};

	public value class SteamPlayer
	{

	public:
		CSteamID * m_handle = NULL;

		SteamPlayer( CSteamID * handle ) :
			m_handle( handle )
		{
		}

		SteamPlayer( uint64 handle ) :
			SteamPlayer( new CSteamID( handle ) )
		{
		}

		UInt64 Id();
		String^ Name();
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

		STEAM_CALLBACK( CallbackClass, OnChatMsg, LobbyChatMsg_t, m_OnChatMsg );
		STEAM_CALLBACK( CallbackClass, OnDataUpdate, LobbyDataUpdate_t, m_OnDataUpdate );
		STEAM_CALLBACK( CallbackClass, OnChatUpdate, LobbyChatUpdate_t, m_OnChatUpdate );

		STEAM_CALLBACK( CallbackClass, OnP2PSessionRequest, P2PSessionRequest_t, m_CallbackP2PSessionRequest );
		STEAM_CALLBACK( CallbackClass, OnP2PSessionConnectFail, P2PSessionConnectFail_t, m_CallbackP2PSessionConnectFail );
	};

	// Global items. Not exposed to the C# wrapper.
	CallbackClass* g_CallbackClassInstance;
	CCallResult< CallbackClass, LeaderboardFindResult_t > g_CallResultFindLeaderboard;
	CCallResult< CallbackClass, LeaderboardScoresDownloaded_t > g_CallResultDownloadEntries;
	CCallResult< CallbackClass, LobbyMatchList_t > g_CallResultLobbyMatchList;
	CCallResult< CallbackClass, LobbyEnter_t > g_CallResultJoinLobby;
	CCallResult< CallbackClass, LobbyCreated_t > g_CallResultLobbyCreated;
	const int nMaxLeaderboardEntries = 1000;
	LeaderboardEntry_t m_leaderboardEntries[nMaxLeaderboardEntries];
	const int nMaxFriendLobbies = 1000;
	CSteamID m_friendLobbies[nMaxFriendLobbies];


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
		static Action^ s_OnChatUpdate;
		static Action< String^, uint64, String^ >^ s_OnChatMsg;
		static Action^ s_OnDataUpdate;

		static int s_nLobbiesFound = 0;
		static int s_nFriendLobbiesFound = 0;

		static SteamLobby s_CurrentLobby;

		static Dictionary< String^, String^ >^ s_LocalLobbyData;

	public:
		static const int
			LobbyType_Public = 0,
			LobbyType_FriendsOnly = 1,
			LobbyType_Private = 2;

		static void FindLobbies( Action< bool >^ OnFind );
		static void FindFriendLobbies( Action< bool >^ OnFind );
		
		static int const NumLobbies();
		static String^ GetLobbyData( int Index, String^ Key );
		static void JoinCreatedLobby(            Action< bool >^ OnJoinLobby, Action^ OnChatUpdate, Action< String^, uint64, String^ >^ OnChatMsg, Action^ OnDataUpdate );
		static void JoinLobby( int Index,        Action< bool >^ OnJoinLobby, Action^ OnChatUpdate, Action< String^, uint64, String^ >^ OnChatMsg, Action^ OnDataUpdate );
		static void JoinLobby( CSteamID LobbyID, Action< bool >^ OnJoinLobby, Action^ OnChatUpdate, Action< String^, uint64, String^ >^ OnChatMsg, Action^ OnDataUpdate );
		static void SetLobbyCallbacks( Action< bool >^ OnJoinLobby, Action^ OnChatUpdate, Action< String^, uint64, String^ >^ OnChatMsg, Action^ OnDataUpdate );

		static void CreateLobby(Action< bool >^ OnCreateLobby, int LobbyType );

		static void SetLobbyData( String^ Key, String^ Value );
		static String^ GetLobbyData( String^ Key );

		static void SendChatMsg( String^ Msg );
		
		static int GetLobbyMemberCount( int Index );
		static int GetLobbyCapacity( int Index );
		static void SetLobbyType( int LobbyType );

		static CSteamID GetLobby( int Index );
		static int GetLobbyMemberCount();
		static String^ GetMememberName( int Index );
		static UInt64 GetMememberId(int Index);

		static bool IsLobbyOwner();

		static void LeaveLobby();
		static void SetLobbyJoinable( bool Joinable );
	};

	public ref class SteamP2P
	{

	internal:

		static Action< uint64 >^ OnRequest;
		static Action< uint64 >^ OnConnectionFail;

	public:

		static void SendMessage( SteamPlayer User, String^ Message );
		static void SendMessage( CSteamID User, String^ Message );
		static bool MessageAvailable();
		static Tuple< UInt64, String^ >^ ReadMessage();

		static void SetOnP2PSessionRequest( Action< uint64 >^ OnRequest );
		static void SetOnP2PSessionConnectFail( Action< uint64 >^ OnConnectionFail );
		static void AcceptP2PSessionWithPlayer( SteamPlayer Player );
	};
}
