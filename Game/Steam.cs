using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using Steamworks;

namespace Steamworks
{
    public class CallbackClass
    {
        public static CallbackClass instance = new CallbackClass();
        public CallbackClass() { }

        public void OnFindLeaderboard(LeaderboardFindResult_t pResult, bool bIOFailure)
        {
            LeaderboardHandle handle = new LeaderboardHandle(pResult.m_hSteamLeaderboard);
            SteamStats.s_OnFind.Invoke(handle, bIOFailure);
        }

        public void OnFindLobbies(LobbyMatchList_t pLobbyMatchList, bool bIOFailure)
        {
            if (!bIOFailure)
            {
                SteamMatches.s_nLobbiesFound = (int)pLobbyMatchList.m_nLobbiesMatching;
            }
            else
            {
                SteamMatches.s_nLobbiesFound = 0;
            }

            SteamMatches.s_OnFindLobbies.Invoke(bIOFailure);
        }

        public void OnJoinLobby(LobbyEnter_t pCallback, bool bIOFailure)
        {
            if (!bIOFailure && !SteamCore.InOfflineMode())
            {
                SteamMatches.s_CurrentLobby = new SteamLobby(pCallback.m_ulSteamIDLobby);
            }

            SteamMatches.s_OnJoinLobby.Invoke(bIOFailure);
        }

        public void OnChatUpdate(LobbyChatUpdate_t pCallback)
        {
            if (SteamMatches.s_OnChatUpdate != null)
            {
                int ChatMemberStateChange = 0;
                var state = pCallback.m_rgfChatMemberStateChange;
                if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeBanned) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Banned;
                else if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeDisconnected) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Disconnected;
                else if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Entered;
                else if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeKicked) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Kicked;
                else if ((state & (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft) != 0)
                    ChatMemberStateChange = SteamMatches.ChatMember_Left;

                SteamMatches.s_OnChatUpdate.Invoke(ChatMemberStateChange, pCallback.m_ulSteamIDUserChanged);
            }
        }

        public void OnChatMsg(LobbyChatMsg_t pCallback)
        {
            CSteamID sender;
            EChatEntryType entryType;

            byte[] pvData = new byte[4096];

            int readed = NativeMethods.ISteamMatchmaking_GetLobbyChatEntry(
                (CSteamID)pCallback.m_ulSteamIDLobby,
                (int)pCallback.m_iChatID,
                out sender,
                pvData,
                pvData.Length,
                out entryType
            );

            if (SteamMatches.s_OnChatMsg != null)
            {
                string msg = StringHelper.GetString(pvData);
                //var id = sender.ConvertToUint64();
                //var pchName = NativeMethods.ISteamFriends_GetFriendPersonaName (sender);
                ulong id = 5;
                string name = "Hello";

                SteamMatches.s_OnChatMsg.Invoke(msg, id, name);
            }
        }

        public void OnDataUpdate(LobbyDataUpdate_t pCallback)
        {
            if (SteamMatches.s_OnDataUpdate != null)
            {
                SteamMatches.s_OnDataUpdate.Invoke();
            }
        }

        public void OnLeaderboardDownloadedEntries(LeaderboardScoresDownloaded_t pLeaderboardScoresDownloaded, bool bIOFailure)
        {
            if (!bIOFailure)
            {
                SteamStats.s_nLeaderboardEntriesFound = Math.Min(pLeaderboardScoresDownloaded.m_cEntryCount, 1000);

                for (int index = 0; index < SteamStats.s_nLeaderboardEntriesFound; index++)
                {
                    NativeMethods.ISteamUserStats_GetDownloadedLeaderboardEntry(
                        pLeaderboardScoresDownloaded.m_hSteamLeaderboardEntries, index, out SteamStats.m_leaderboardEntries[index], null, 0);
                }
            }

            SteamStats.s_OnDownload.Invoke(bIOFailure);
        }

        public void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure)
        {
            if (!bIOFailure)
            {
                SteamMatches.s_CurrentLobby = new SteamLobby(pCallback.m_ulSteamIDLobby);
            }

            SteamMatches.s_OnCreateLobby.Invoke(bIOFailure);
        }


        //public STEAM_CALLBACK(CallbackClass, OnChatMsg, LobbyChatMsg_t, m_OnChatMsg)
        //{
        //    //
        //}

        // There are like a dozen more STEAM_CALLBACK things
    }

    public struct LeaderboardHandle
    {
        public SteamLeaderboard_t m_handle;

        public LeaderboardHandle(SteamLeaderboard_t handle)
        {
            m_handle = handle;
        }
    }

    public static class SteamCore
    {
        static bool s_bOffline;

        public static bool Initialize()
        {
            if (!SteamAPI.Init())
                return false;

            if (!SteamStats.Initialize())
                return false;

            return true;
        }

        public static bool InOfflineMode()
        {
            return s_bOffline;
        }

        public static UInt64 PlayerId()
        {
            if (InOfflineMode() || !SteamIsConnected())
            {
                // Return consistent but invalid Steam ID, for use throughout the application.
                return 12345;
            }

            return (UInt64)SteamUser.GetSteamID();
        }

        public static String PlayerName()
        {
            InteropHelp.TestIfAvailableClient();
            return InteropHelp.PtrToStringUTF8(NativeMethods.ISteamFriends_GetPersonaName());
        }

        public static bool RestartViaSteamIfNecessary(AppId_t AppId)
        {
            bool result = NativeMethods.SteamAPI_RestartAppIfNecessary(AppId);
            return result;
        }

        public static void SetOfflineMode(bool Offline)
        {
            s_bOffline = Offline;
        }

        public static void Shutdown()
        {
            NativeMethods.SteamAPI_Shutdown();
        }

        public static bool SteamIsConnected()
        {
            return SteamIsRunning() && NativeMethods.ISteamUser_BLoggedOn();
        }

        public static bool SteamIsRunning()
        {
            return NativeMethods.SteamAPI_IsSteamRunning();
        }

        public static void Update()
        {
            NativeMethods.SteamAPI_RunCallbacks();
        }
    }

    public struct SteamLobby
    {
        public CSteamID m_handle;

        public SteamLobby(CSteamID handle)
        {
            m_handle = handle;
        }

        public SteamLobby(ulong handle)
        {
            m_handle = (CSteamID)handle;
        }
    }

    public static class StringHelper
    {
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes, int length = 0)
        {
            if (length == 0)
                length = bytes.Length;

            char[] chars = new char[length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, length);
            return new string(chars);
        }
    }

    public static class SteamMatches
    {
        public static Action<bool> s_OnFindLobbies;
        public static Action<bool> s_OnJoinLobby;
        public static Action<bool> s_OnCreateLobby;
        public static Action<int, UInt64> s_OnChatUpdate;
        public static Action<String, UInt64, String> s_OnChatMsg;
        public static Action s_OnDataUpdate;

        public static int s_nLobbiesFound = 0;
        public static int s_nFriendLobbiesFound = 0;

        public static SteamLobby s_CurrentLobby;
        public static CallResult<LeaderboardFindResult_t> g_CallResultFindLeaderboard;
        public static CallResult<LeaderboardScoresDownloaded_t> g_CallResultDownloadEntries;
        public static CallResult<LobbyMatchList_t> g_CallResultLobbyMatchList;
        public static CallResult<LobbyEnter_t> g_CallResultJoinLobby;
        public static CallResult<LobbyCreated_t> g_CallResultLobbyCreated;

        public static Callback<LobbyChatMsg_t> g_CallResultChatMsg;
        public static Callback<LobbyDataUpdate_t> g_CallResultDataUpdate;
        public static Callback<LobbyChatUpdate_t> g_CallResultChatUpdate;
        public static Callback<P2PSessionRequest_t> g_CallResultP2PSessionRequest;
        public static Callback<P2PSessionConnectFail_t> g_CallResultP2PSessionConnectFail;

        public static Dictionary<String, String> s_LocalLobbyData;
        const int nMaxFriendLobbies = 1000;
        static CSteamID[] m_friendLobbies = new CSteamID[nMaxFriendLobbies];

        public const int
            LobbyType_Public = 0,
            LobbyType_FriendsOnly = 1,
            LobbyType_Private = 2;

        public const int
            ChatMember_Entered = 1,      // This user has joined or is joining the chat room
            ChatMember_Left = 2,         // This user has left or is leaving the chat room
            ChatMember_Disconnected = 3, // User disconnected without leaving the chat first
            ChatMember_Kicked = 4,       // User kicked
            ChatMember_Banned = 5;       // User kicked and banned

        public static void FindLobbies(Action<bool> OnFind)
        {
            s_OnFindLobbies = OnFind;
            s_nLobbiesFound = 0;
            s_nFriendLobbiesFound = 0;

            //if ( SteamMatchmaking() == 0 ) return;
            InteropHelp.TestIfAvailableClient();

            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)NativeMethods.ISteamMatchmaking_RequestLobbyList();
            g_CallResultLobbyMatchList = new CallResult<LobbyMatchList_t>(CallbackClass.instance.OnFindLobbies);
            g_CallResultLobbyMatchList.Set(hSteamAPICall); //( hSteamAPICall, SteamStats.g_CallbackClassInstance, CallbackClass.OnFindLobbies );
        }

        public static void FindFriendLobbies(Action<bool> OnFind)
        {
            s_OnFindLobbies = OnFind;
            s_nLobbiesFound = 0;
            s_nFriendLobbiesFound = 0;

            //if ( SteamMatchmaking() == 0 ) return;
            InteropHelp.TestIfAvailableClient();

            int cFriends = NativeMethods.ISteamFriends_GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            for (int i = 0; i < cFriends; i++)
            {
                FriendGameInfo_t friendGameInfo;
                CSteamID steamIDFriend = (CSteamID)NativeMethods.ISteamFriends_GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                if (NativeMethods.ISteamFriends_GetFriendGamePlayed(steamIDFriend, out friendGameInfo) && friendGameInfo.m_steamIDLobby.IsValid())
                {
                    m_friendLobbies[s_nFriendLobbiesFound++] = friendGameInfo.m_steamIDLobby;
                    NativeMethods.ISteamMatchmaking_RequestLobbyData(friendGameInfo.m_steamIDLobby);
                    //int cap = SteamMatchmaking().GetLobbyMemberLimit( friendGameInfo.m_steamIDLobby );
                    //Console.WriteLine("Found friend lobby with capacity {0}", cap);
                }
            }

            if (s_OnFindLobbies != null)
            {
                s_OnFindLobbies(false);
            }
        }

        public static int NumLobbies()
        {
            if (s_nFriendLobbiesFound > 0)
            {
                return s_nFriendLobbiesFound;
            }
            else
            {
                return s_nLobbiesFound;
            }
        }

        public static CSteamID GetLobby(int Index)
        {
            if (s_nFriendLobbiesFound > 0)
            {
                return m_friendLobbies[Index];
            }
            else
            {
                return (CSteamID)NativeMethods.ISteamMatchmaking_GetLobbyByIndex(Index);
            }
        }

        public static string GetLobbyData(int Index, string Key)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            InteropHelp.TestIfAvailableClient();
            using (var Key2 = new InteropHelp.UTF8StringHandle(Key))
            {
                return InteropHelp.PtrToStringUTF8(NativeMethods.ISteamMatchmaking_GetLobbyData(steamIDLobby, Key2));
            }
        }

        public static void JoinCreatedLobby(
            Action<bool> OnJoinLobby,
            Action<int, UInt64> OnChatUpdate,
            Action<String, UInt64, String> OnChatMsg,
            Action OnDataUpdate)
        {
            if (SteamCore.InOfflineMode())
            {
                SetLobbyCallbacks(OnJoinLobby, OnChatUpdate, OnChatMsg, OnDataUpdate);
                s_OnJoinLobby.Invoke(false);
                return;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            JoinLobby(s_CurrentLobby.m_handle, OnJoinLobby, OnChatUpdate, OnChatMsg, OnDataUpdate);
        }

        public static void JoinLobby(int Index,
            Action<bool> OnJoinLobby,
            Action<int, UInt64> OnChatUpdate,
            Action<String, UInt64, String> OnChatMsg,
            Action OnDataUpdate)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            JoinLobby(steamIDLobby, OnJoinLobby, OnChatUpdate, OnChatMsg, OnDataUpdate);
        }

        public static void JoinLobby(CSteamID LobbyID,
            Action<bool> OnJoinLobby,
            Action<int, UInt64> OnChatUpdate,
            Action<String, UInt64, String> OnChatMsg,
            Action OnDataUpdate)
        {
            SetLobbyCallbacks(OnJoinLobby, OnChatUpdate, OnChatMsg, OnDataUpdate);

            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)NativeMethods.ISteamMatchmaking_JoinLobby(LobbyID);
            g_CallResultJoinLobby.Set(hSteamAPICall);
        }

        public static void SetLobbyCallbacks(
            Action<bool> OnJoinLobby,
            Action<int, UInt64> OnChatUpdate,
            Action<String, UInt64, String> OnChatMsg,
            Action OnDataUpdate)
        {
            s_OnJoinLobby = OnJoinLobby;
            s_OnChatUpdate = OnChatUpdate;
            s_OnChatMsg = OnChatMsg;
            s_OnDataUpdate = OnDataUpdate;

            g_CallResultJoinLobby = new CallResult<LobbyEnter_t>(CallbackClass.instance.OnJoinLobby);
            g_CallResultChatUpdate = new Callback<LobbyChatUpdate_t>(CallbackClass.instance.OnChatUpdate);
            g_CallResultChatMsg = new Callback<LobbyChatMsg_t>(CallbackClass.instance.OnChatMsg);
            g_CallResultDataUpdate = new Callback<LobbyDataUpdate_t>(CallbackClass.instance.OnDataUpdate);
        }

        public static ELobbyType IntToLobbyType(int LobbyType)
        {
            ELobbyType type = ELobbyType.k_ELobbyTypePublic;

            switch (LobbyType)
            {
                case LobbyType_Public: type = ELobbyType.k_ELobbyTypePublic; break;
                case LobbyType_FriendsOnly: type = ELobbyType.k_ELobbyTypeFriendsOnly; break;
                case LobbyType_Private: type = ELobbyType.k_ELobbyTypePrivate; break;
            }

            return type;
        }

        public static void CreateLobby(Action<bool> OnCreateLobby, int LobbyType)
        {
            if (SteamCore.InOfflineMode())
            {
                s_LocalLobbyData = new Dictionary<String, String>();

                OnCreateLobby(false);

                return;
            }

            s_OnCreateLobby = OnCreateLobby;

            ELobbyType type = IntToLobbyType(LobbyType);

            InteropHelp.TestIfAvailableClient();
            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)NativeMethods.ISteamMatchmaking_CreateLobby(type, 4);
            g_CallResultLobbyCreated = new CallResult<LobbyCreated_t>(CallbackClass.instance.OnLobbyCreated);
            g_CallResultLobbyCreated.Set(hSteamAPICall); //, SteamStats.g_CallbackClassInstance, CallbackClass.OnLobbyCreated );
        }

        public static void SetLobbyData(string Key, string Value)
        {
            if (SteamCore.InOfflineMode())
            {
                s_LocalLobbyData[Key] = Value;
                //REMOVE//SteamStats.g_CallbackClassInstance.OnDataUpdate(null);
                return;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            using (var pchKey2 = new InteropHelp.UTF8StringHandle(Key))
            using (var pchValue2 = new InteropHelp.UTF8StringHandle(Value))
            {
                NativeMethods.ISteamMatchmaking_SetLobbyData(s_CurrentLobby.m_handle, pchKey2, pchValue2);
            }
        }

        public static string GetLobbyData(string Key)
        {
            if (SteamCore.InOfflineMode())
            {
                try
                {
                    return s_LocalLobbyData[Key];
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return "";

            using (var pchKey2 = new InteropHelp.UTF8StringHandle(Key))
            {
                return InteropHelp.PtrToStringUTF8(NativeMethods.ISteamMatchmaking_GetLobbyData(s_CurrentLobby.m_handle, pchKey2));
            }
        }

        public static int GetLobbyMemberCount(int Index)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            return NativeMethods.ISteamMatchmaking_GetNumLobbyMembers(steamIDLobby);
        }

        public static int GetLobbyCapacity(int Index)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            return NativeMethods.ISteamMatchmaking_GetLobbyMemberLimit(steamIDLobby);
        }

        public static bool SetLobbyMemberLimit(int MaxMembers)
        {
            if (SteamCore.InOfflineMode()) return false;
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return false;

            return NativeMethods.ISteamMatchmaking_SetLobbyMemberLimit(s_CurrentLobby.m_handle, MaxMembers);
        }

        public static void SetLobbyType(int LobbyType)
        {
            if (SteamCore.InOfflineMode()) return;
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            ELobbyType type = IntToLobbyType(LobbyType);
            NativeMethods.ISteamMatchmaking_SetLobbyType(s_CurrentLobby.m_handle, type);
        }

        public static void SendChatMsg(string Msg)
        {
            if (SteamCore.InOfflineMode())
            {
                s_OnChatMsg.Invoke(Msg, SteamCore.PlayerId(), "player");
                return;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            //marshal_context context;
            //char   pchMsg = context.marshal_as< char   >( Msg );

            var bytes = StringHelper.GetBytes(Msg);

            using (var pchMsg2 = new InteropHelp.UTF8StringHandle(Msg))
            {
                NativeMethods.ISteamMatchmaking_SendLobbyChatMsg(s_CurrentLobby.m_handle, bytes, bytes.Length);
            }
        }

        public static int GetLobbyMemberCount()
        {
            if (SteamCore.InOfflineMode())
            {
                return 1;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return -1;
            return NativeMethods.ISteamMatchmaking_GetNumLobbyMembers(s_CurrentLobby.m_handle);
        }

        public static String GetMememberName(int Index)
        {
            if (SteamCore.InOfflineMode())
            {
                return "Local player";
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return "";

            CSteamID steamIDLobbyMember = (CSteamID)NativeMethods.ISteamMatchmaking_GetLobbyMemberByIndex(s_CurrentLobby.m_handle, Index);

            return InteropHelp.PtrToStringUTF8(NativeMethods.ISteamFriends_GetFriendPersonaName(steamIDLobbyMember));
        }

        public static UInt64 GetMememberId(int Index)
        {
            if (SteamCore.InOfflineMode())
            {
                return SteamCore.PlayerId();
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return 0;

            CSteamID steamIDLobbyMember = (CSteamID)NativeMethods.ISteamMatchmaking_GetLobbyMemberByIndex(s_CurrentLobby.m_handle, Index);

            return (UInt64)steamIDLobbyMember;
        }

        public static bool IsLobbyOwner()
        {
            if (SteamCore.InOfflineMode()) return true;

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return false;
            return (CSteamID)NativeMethods.ISteamUser_GetSteamID() == (CSteamID)NativeMethods.ISteamMatchmaking_GetLobbyOwner(s_CurrentLobby.m_handle);
        }

        public static void LeaveLobby()
        {
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            NativeMethods.ISteamMatchmaking_LeaveLobby(s_CurrentLobby.m_handle);
            s_CurrentLobby.m_handle.Clear();
        }

        public static void SetLobbyJoinable(bool Joinable)
        {
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            NativeMethods.ISteamMatchmaking_SetLobbyJoinable(s_CurrentLobby.m_handle, Joinable);
        }
    }

    public static class SteamP2P
    {
        public static Action<UInt64> OnRequest;
        public static Action<UInt64> OnConnectionFail;

        public static void SendMessage(SteamPlayer User, String Message)
        {
            SendMessage(User.m_handle, Message);
        }

        public static void SendMessage(CSteamID User, String Message)
        {
            byte[] pchMsg = new byte[Message.Length];

            NativeMethods.ISteamGameServerNetworking_SendP2PPacket(User, pchMsg, (uint)Message.Length, EP2PSend.k_EP2PSendReliable, 0);
        }

        public static void SendBytes(SteamPlayer User, byte[] Bytes)
        {
            SendBytes(User.m_handle, Bytes);
        }

        public static void SendBytes(CSteamID User, byte[] Bytes)
        {
            uint len = (uint)Bytes.Length;

            byte[] pchMsg = new byte[len];
            for (int i = 0; i < len; i++)
            {
                pchMsg[i] = Bytes[i];
            }

            //int count = 0;
            //byte   pchMsg = new byte[len];
            //for (int i = 0; i < len; i++)
            //{
            //    if (Bytes[i] == 0) continue;

            //    pchMsg[count] = Bytes[i];
            //    count++;
            //}
            //len = count;

            NativeMethods.ISteamGameServerNetworking_SendP2PPacket(User, pchMsg, len, EP2PSend.k_EP2PSendReliable, 0);

            //delete[] pchMsg;
        }

        public static bool MessageAvailable()
        {
            //UInt32 msgSize = 0;
            bool result = false;
            //bool result = SteamNetworking().IsP2PPacketAvailable( msgSize );

            //if ( result )
            //{
            //    return result;
            //}

            return result;
        }

        public static Tuple<UInt64, String> ReadMessage()
        {
            UInt32 msgSize = 0;
            bool result = NativeMethods.ISteamGameServerNetworking_IsP2PPacketAvailable(out msgSize, 0);

            if (!result)
            {
                return null;
            }

            var packet = new byte[msgSize];
            string msg = "";
            CSteamID steamIDRemote;
            UInt32 bytesRead = 0;

            if (NativeMethods.ISteamGameServerNetworking_ReadP2PPacket(packet, msgSize, out bytesRead, out steamIDRemote, 0))
            {
                msg = packet.ToString();
            }

            return new Tuple<UInt64, String>((ulong)steamIDRemote.GetAccountID(), msg);
        }

        public static Tuple<UInt64, byte[]> ReadBytes()
        {
            UInt32 msgSize = 0;
            bool result = NativeMethods.ISteamGameServerNetworking_IsP2PPacketAvailable(out msgSize, 0);

            if (!result)
            {
                return null;
            }

            var packet = new byte[msgSize];
            CSteamID steamIDRemote;
            UInt32 bytesRead = 0;

            byte[] Bytes = new byte[msgSize];
            if (NativeMethods.ISteamGameServerNetworking_ReadP2PPacket(packet, msgSize, out bytesRead, out steamIDRemote, 0))
            {
                for (int i = 0; i < msgSize; i++)
                {
                    Bytes[i] = packet[i];
                }
            }

            return new Tuple<UInt64, byte[]>((UInt64)steamIDRemote, Bytes);
        }

        public static void SetOnP2PSessionRequest(Action<UInt64> OnRequest)
        {
            SteamP2P.OnRequest = OnRequest;
        }

        public static void SetOnP2PSessionConnectFail(Action<UInt64> OnConnectionFail)
        {
            SteamP2P.OnConnectionFail = OnConnectionFail;
        }

        public static void AcceptP2PSessionWithPlayer(SteamPlayer Player)
        {
            NativeMethods.ISteamGameServerNetworking_AcceptP2PSessionWithUser(Player.m_handle);
        }
    }

    public class SteamPlayer
    {
        public CSteamID m_handle;

        public SteamPlayer(CSteamID handle)
        {
            m_handle = handle;
        }

        public SteamPlayer(UInt64 handle)
        {
            m_handle = new CSteamID(handle);
        }
        public UInt64 Id()
        {
            return (UInt64)m_handle;
        }

        public String Name()
        {
            InteropHelp.TestIfAvailableClient();
            return InteropHelp.PtrToStringUTF8(NativeMethods.ISteamFriends_GetFriendPersonaName(m_handle));
        }
    }

    public class SteamStats
    {

        public static int s_nLeaderboardEntriesFound = 0;
        public static Action<LeaderboardHandle, bool> s_OnFind;
        public static Action<bool> s_OnDownload;

        const int nMaxLeaderboardEntries = 1000;
        static public LeaderboardEntry_t[] m_leaderboardEntries = new LeaderboardEntry_t[nMaxLeaderboardEntries];
        public static CallbackClass g_CallbackClassInstance;

        public static bool Initialize()
        {
            //REMOVE//g_CallbackClassInstance = new CallbackClass();

            // Is Steam loaded? If not we can't get stats.
            InteropHelp.TestIfAvailableClient();
            SteamAPI.Init();
            //if ( SteamUserStats() == 0 )
            ////if ( SteamUserStats() == 0 || SteamUser() == 0 )
            //{
            //    return false;
            //}
            // Is the user logged on?  If not we can't get stats.
            //if ( !SteamUser().BLoggedOn() )
            //{
            //    return false;
            //}
            // Request user stats.
            return NativeMethods.ISteamUserStats_RequestCurrentStats();
        }

        public bool GiveAchievement(string AchievementApiName)
        {
            //SteamUserStats().StoreStats();

            using (var pchName2 = new InteropHelp.UTF8StringHandle(AchievementApiName))
            {
                return NativeMethods.ISteamUserStats_SetAchievement(pchName2);
            }
        }

        public int NumEntries(SteamLeaderboard_t hSteamLeaderboard)
        {
            return NativeMethods.ISteamUserStats_GetLeaderboardEntryCount(hSteamLeaderboard);
        }

        public int NumEntriesFound()
        {
            return s_nLeaderboardEntriesFound;
        }

        public void FindLeaderboard(string LeaderboardName, Action<LeaderboardHandle, bool> OnFind)
        {
            //marshal_context context;
            // char pchLeaderboardName = context.marshal_as<  char >( LeaderboardName );

            s_OnFind = OnFind;

            InteropHelp.TestIfAvailableClient();

            //if( SteamUserStats() == 0 )
            //{
            //    return;
            //}

            using (var pchLeaderboardName2 = new InteropHelp.UTF8StringHandle(LeaderboardName))
            {
                SteamAPICall_t hSteamAPICall = (SteamAPICall_t)NativeMethods.ISteamUserStats_FindLeaderboard(pchLeaderboardName2);
                SteamMatches.g_CallResultFindLeaderboard = new CallResult<LeaderboardFindResult_t>(CallbackClass.instance.OnFindLeaderboard);
                SteamMatches.g_CallResultFindLeaderboard.Set(hSteamAPICall); //, SteamStats.g_CallbackClassInstance, CallbackClass.OnFindLeaderboard);
            }
        }

        public void UploadScore(LeaderboardHandle Handle, int Value)
        {
            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)NativeMethods.ISteamUserStats_UploadLeaderboardScore(Handle.m_handle, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, Value, null, 0);
        }

        public void RequestEntries(LeaderboardHandle Handle, int RequestType, int Start, int End, Action<bool> OnDownload)
        {
            s_OnDownload = OnDownload;

            // Request the specified leaderboard data.
            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)NativeMethods.ISteamUserStats_DownloadLeaderboardEntries(
                Handle.m_handle, (ELeaderboardDataRequest)(RequestType), Start, End);

            // Register for the async callback
            SteamMatches.g_CallResultDownloadEntries = new CallResult<LeaderboardScoresDownloaded_t>(CallbackClass.instance.OnLeaderboardDownloadedEntries);
            SteamMatches.g_CallResultDownloadEntries.Set(hSteamAPICall); //, SteamStats.g_CallbackClassInstance, CallbackClass.OnLeaderboardDownloadedEntries);
        }

        public int Results_GetScore(int Index)
        {
            return m_leaderboardEntries[Index].m_nScore;
        }

        public int Results_GetRank(int Index)
        {
            return m_leaderboardEntries[Index].m_nGlobalRank;
        }

        public string Results_GetPlayer(int Index)
        {
            return InteropHelp.PtrToStringUTF8(NativeMethods.ISteamFriends_GetFriendPersonaName(m_leaderboardEntries[Index].m_steamIDUser));
        }

        public int Results_GetId(int Index)
        {
            return (int)m_leaderboardEntries[Index].m_steamIDUser.GetAccountID().m_AccountID;
        }
    }

    public class SteamTextInput
    {
        public static Action<bool> s_OnGamepadInputEnd;
        public string GetText()
        {
            uint cchText = NativeMethods.ISteamUtils_GetEnteredGamepadTextLength();
            IntPtr pchText = new IntPtr();

            NativeMethods.ISteamUtils_GetEnteredGamepadTextInput(pchText, cchText);

            return pchText.ToString();
        }

        public bool ShowGamepadTextInput(string Description, string InitialText, uint MaxCharacters, Action<bool> OnGamepadInputEnd)
        {
            s_OnGamepadInputEnd = OnGamepadInputEnd;

            //marshal_context context;
            // char pchDescription = context.marshal_as<  char >( Description );
            // char pchInitialText = context.marshal_as<  char >( InitialText );

            UInt64 unCharMax = (UInt64)(MaxCharacters);

            using (var pchDescription = new InteropHelp.UTF8StringHandle(Description))
            using (var pchInitialText = new InteropHelp.UTF8StringHandle(InitialText))
            {
                bool val = false; // delete this line after the REMOVE is taken out
                                  //REMOVE//bool val = NativeMethods.ISteamGameServerUtils_ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, pchDescription, unCharMax, pchInitialText);

                return val;
            }
        }
    }
}

