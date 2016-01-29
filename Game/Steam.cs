using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Steamworks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;

namespace SteamWrapper
{
    public class SteamHtml
    {
        public static bool AllowMouseEvents = true;

        public static HHTMLBrowser Browser;
        public static Texture2D Texture;
        static byte[] pixels;

        static CallResult<HTML_BrowserReady_t> g_CallResultBrowserReady;

        static Callback<HTML_CloseBrowser_t> Event_CloseBrowser;
        static Callback<HTML_NeedsPaint_t> Event_NeedsPaint;
        static Callback<HTML_StartRequest_t> Event_StartRequest;
        static Callback<HTML_FinishedRequest_t> Event_FinishedRequest;
        static Callback<HTML_URLChanged_t> Event_URLChanged;

        static Keys[] lastPressedKeys;
        static Keys[] currentPressedKeys = new Keys[0];

        static uint Width, Height;

        public static void Initialize(uint _Width, uint _Height)
        {
            Width = _Width;
            Height = _Height;

            bool result = SteamHTMLSurface.Init();

            SteamHTMLSurface.SetSize(HHTMLBrowser.Invalid, Width, Height);
            Texture = new Texture2D(Game.GameClass.Graphics, (int)Width, (int)Height);
            pixels = new byte[Width * Height * 4];

            var hSteamAPICall = SteamHTMLSurface.CreateBrowser("WAL", null);
            g_CallResultBrowserReady = new CallResult<HTML_BrowserReady_t>(OnBrowserReady);
            g_CallResultBrowserReady.Set(hSteamAPICall);

            Event_CloseBrowser = new Callback<HTML_CloseBrowser_t>(OnCloseBrowser);
            Event_StartRequest = new Callback<HTML_StartRequest_t>(OnStartRequest);
            Event_FinishedRequest = new Callback<HTML_FinishedRequest_t>(OnFinishedRequest);
            Event_NeedsPaint = new Callback<HTML_NeedsPaint_t>(OnNeedsPaint);
            Event_URLChanged = new Callback<HTML_URLChanged_t>(OnURLChange);
        }

        public static char KeyToChar(Keys key)
        {
            bool shift = Keys.LeftShift.Down() || Keys.RightShift.Down();

            switch (key)
            {
                //Alphabet keys
                case Keys.A: if (shift) { return 'A'; } else { return 'a'; }
                case Keys.B: if (shift) { return 'B'; } else { return 'b'; }
                case Keys.C: if (shift) { return 'C'; } else { return 'c'; }
                case Keys.D: if (shift) { return 'D'; } else { return 'd'; }
                case Keys.E: if (shift) { return 'E'; } else { return 'e'; }
                case Keys.F: if (shift) { return 'F'; } else { return 'f'; }
                case Keys.G: if (shift) { return 'G'; } else { return 'g'; }
                case Keys.H: if (shift) { return 'H'; } else { return 'h'; }
                case Keys.I: if (shift) { return 'I'; } else { return 'i'; }
                case Keys.J: if (shift) { return 'J'; } else { return 'j'; }
                case Keys.K: if (shift) { return 'K'; } else { return 'k'; }
                case Keys.L: if (shift) { return 'L'; } else { return 'l'; }
                case Keys.M: if (shift) { return 'M'; } else { return 'm'; }
                case Keys.N: if (shift) { return 'N'; } else { return 'n'; }
                case Keys.O: if (shift) { return 'O'; } else { return 'o'; }
                case Keys.P: if (shift) { return 'P'; } else { return 'p'; }
                case Keys.Q: if (shift) { return 'Q'; } else { return 'q'; }
                case Keys.R: if (shift) { return 'R'; } else { return 'r'; }
                case Keys.S: if (shift) { return 'S'; } else { return 's'; }
                case Keys.T: if (shift) { return 'T'; } else { return 't'; }
                case Keys.U: if (shift) { return 'U'; } else { return 'u'; }
                case Keys.V: if (shift) { return 'V'; } else { return 'v'; }
                case Keys.W: if (shift) { return 'W'; } else { return 'w'; }
                case Keys.X: if (shift) { return 'X'; } else { return 'x'; }
                case Keys.Y: if (shift) { return 'Y'; } else { return 'y'; }
                case Keys.Z: if (shift) { return 'Z'; } else { return 'z'; }

                //Decimal keys
                case Keys.D0: if (shift) { return ')'; } else { return '0'; }
                case Keys.D1: if (shift) { return '!'; } else { return '1'; }
                case Keys.D2: if (shift) { return '@'; } else { return '2'; }
                case Keys.D3: if (shift) { return '#'; } else { return '3'; }
                case Keys.D4: if (shift) { return '$'; } else { return '4'; }
                case Keys.D5: if (shift) { return '%'; } else { return '5'; }
                case Keys.D6: if (shift) { return '^'; } else { return '6'; }
                case Keys.D7: if (shift) { return '&'; } else { return '7'; }
                case Keys.D8: if (shift) { return '*'; } else { return '8'; }
                case Keys.D9: if (shift) { return '('; } else { return '9'; }

                //Decimal numpad keys
                case Keys.NumPad0: return '0';
                case Keys.NumPad1: return '1';
                case Keys.NumPad2: return '2';
                case Keys.NumPad3: return '3';
                case Keys.NumPad4: return '4';
                case Keys.NumPad5: return '5';
                case Keys.NumPad6: return '6';
                case Keys.NumPad7: return '7';
                case Keys.NumPad8: return '8';
                case Keys.NumPad9: return '9';

                //Special keys
                case Keys.OemTilde: if (shift) { return '~'; } else { return '`'; }
                case Keys.OemSemicolon: if (shift) { return ':'; } else { return ';'; }
                case Keys.OemQuotes: if (shift) { return '"'; } else { return '\''; }
                case Keys.OemQuestion: if (shift) { return '?'; } else { return '/'; }
                case Keys.OemPlus: if (shift) { return '+'; } else { return '='; }
                case Keys.OemPipe: if (shift) { return '|'; } else { return '\\'; }
                case Keys.OemPeriod: if (shift) { return '>'; } else { return '.'; }
                case Keys.OemOpenBrackets: if (shift) { return '{'; } else { return '['; }
                case Keys.OemCloseBrackets: if (shift) { return '}'; } else { return ']'; }
                case Keys.OemMinus: if (shift) { return '_'; } else { return '-'; }
                case Keys.OemComma: if (shift) { return '<'; } else { return ','; }
                case Keys.Space: return ' ';
            }

            return (char)0;
        }

        public static void Update()
        {
            if (Browser == null) return;

            if (Input.LeftMousePressed) SteamHTMLSurface.MouseDown(Browser, EHTMLMouseButton.eHTMLMouseButton_Left);
            if (Input.LeftMouseReleased) SteamHTMLSurface.MouseUp(Browser, EHTMLMouseButton.eHTMLMouseButton_Left);

            SteamHTMLSurface.MouseMove(Browser, (int)Input.CurMousePos.x, (int)Input.CurMousePos.y);

            lastPressedKeys = currentPressedKeys;
            currentPressedKeys = Keyboard.GetState().GetPressedKeys();

            // Key Down
            foreach (var key in currentPressedKeys)
            {
                if (!lastPressedKeys.Contains(key))
                {
                    uint code = (uint)key;

                    var mod = Keys.LeftShift.Down() || Keys.RightShift.Down() ?
                        EHTMLKeyModifiers.k_eHTMLKeyModifier_ShiftDown : EHTMLKeyModifiers.k_eHTMLKeyModifier_None;

                    SteamHTMLSurface.KeyDown(Browser, code, mod);

                    if (KeyToChar(key) != (char)0)
                    {
                        SteamHTMLSurface.KeyChar(Browser, (uint)KeyToChar(key), EHTMLKeyModifiers.k_eHTMLKeyModifier_None);
                    }
                }
            }

            // Key Up
            foreach (var key in lastPressedKeys)
            {
                if (!currentPressedKeys.Contains(key))
                {
                    uint code = (uint)key;

                    SteamHTMLSurface.KeyUp(Browser, code, EHTMLKeyModifiers.k_eHTMLKeyModifier_None);
                }
            }
        }

        public static void ExecuteJS(string code)
        {
            if (Browser == null) return;

            //Console.WriteLine($"Executing js {code}");
            SteamHTMLSurface.ExecuteJavascript(Browser, code);
        }

        public static void OnBrowserReady(HTML_BrowserReady_t pBrowserReady, bool bIOFailure)
        {
            if (bIOFailure) return;

            Browser = pBrowserReady.unBrowserHandle;
            SteamHTMLSurface.SetSize(Browser, Width, Height);

            var data = "file://" + Environment.CurrentDirectory + @"/html/index.html";

            SteamHTMLSurface.LoadURL(Browser, data, null);
        }

        public static void OnCloseBrowser(HTML_CloseBrowser_t pParam)
        {
            Browser = HHTMLBrowser.Invalid;
        }

        public static void OnNeedsPaint(HTML_NeedsPaint_t pParam)
        {
            Console.WriteLine("Browser needs painting.");

            if (pParam.unWide != Width)
            {
                Console.WriteLine("bad texture width for html\n");
                return;
            }

            if (pParam.unTall != Height)
            {
                Console.WriteLine("bad texture height for html\n");
                return;
            }
            
            Marshal.Copy(pParam.pBGRA, pixels, 0, pixels.Length);
            for (int i = 0; i < pixels.Length; i += 4)
            {
                Game.CoreMath.Swap(ref pixels[i], ref pixels[i + 2]);

                if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                {
                    pixels[i + 3] = 0;
                }
            }

            Texture.SetData(pixels);
        }

        public static int InvocationGuid = -1;
        public static void OnURLChange(HTML_URLChanged_t pParam)
        {
            try
            {
                string invocation = pParam.pchURL.After('#');

                if (!invocation.StartsWith("invoke!")) return;

                string guid_str = invocation.Split('!')[1];
                int guid = int.Parse(guid_str.Substring("guid".Length));

                if (guid <= InvocationGuid) return;
                InvocationGuid = guid;

                invocation = invocation.After('!').After('!');

                //Console.WriteLine($"Invocating {invocation.Abbreviated()}");
                Game.GameClass.Game.ExecuteInvocation(invocation);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error executing function in {pParam.pchURL}.");
            }
        }

        public static void OnStartRequest(HTML_StartRequest_t pParam)
        {
            Console.WriteLine("Browser request initiated.");

            SteamHTMLSurface.AllowStartRequest(Browser, true);
        }

        public static void OnFinishedRequest(HTML_FinishedRequest_t pParam)
        {
            Console.WriteLine("Browser request finished.");

            // Uncomment this if you want to scale a pages contents when you display it
            //SteamHTMLSurface()->SetPageScaleFactor( m_unBrowserHandle, 2.0, 0, 0 );
        }
    }

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

            SteamMatchmaking.GetLobbyChatEntry(
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
                var id = sender.m_SteamID;
                string name = SteamFriends.GetFriendPersonaName(sender);

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
                    SteamUserStats.GetDownloadedLeaderboardEntry(
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
            return SteamFriends.GetPersonaName();
        }

        public static bool RestartViaSteamIfNecessary(uint AppId)
        {
            bool result = SteamAPI.RestartAppIfNecessary((AppId_t)AppId);
            return result;
        }

        public static void SetOfflineMode(bool Offline)
        {
            s_bOffline = Offline;
        }

        public static void Shutdown()
        {
            SteamAPI.Shutdown();
        }

        public static bool SteamIsConnected()
        {
            return SteamIsRunning() && SteamUser.BLoggedOn();
        }

        public static bool SteamIsRunning()
        {
            return SteamAPI.IsSteamRunning();
        }

        public static void Update()
        {
            SteamAPI.RunCallbacks();
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
        public static string Abbreviated(this string str)
        {
            if (str.Length > 50)
            {
                return $"JS log: {str.Substring(0, 50)}...";
            }
            else
            {
                return $"JS log: {str}";
            }
        }

        public static string After(this string str, char c)
        {
            return str.Substring(str.IndexOf(c) + 1);
        }

        public static byte[] GetBytes(string str)
        {
            int msgLength = str.Length * sizeof(char);

            byte[] bytes = new byte[msgLength + 4];
            var length = BitConverter.GetBytes(msgLength);

            System.Buffer.BlockCopy(length, 0, bytes, 0, 4);
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 4, msgLength);

            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            int length = BitConverter.ToInt32(bytes, 0);

            char[] chars = new char[length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 4, chars, 0, length);
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

        public static bool InLobby()
        {
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return false;
            return false;
        }

        public static void FindLobbies(Action<bool> OnFind)
        {
            s_OnFindLobbies = OnFind;
            s_nLobbiesFound = 0;
            s_nFriendLobbiesFound = 0;

            InteropHelp.TestIfAvailableClient();

            var hSteamAPICall = SteamMatchmaking.RequestLobbyList();
            g_CallResultLobbyMatchList = new CallResult<LobbyMatchList_t>(CallbackClass.instance.OnFindLobbies);
            g_CallResultLobbyMatchList.Set(hSteamAPICall);
        }

        public static void FindFriendLobbies(Action<bool> OnFind)
        {
            s_OnFindLobbies = OnFind;
            s_nLobbiesFound = 0;
            s_nFriendLobbiesFound = 0;

            //if ( SteamMatchmaking() == 0 ) return;
            InteropHelp.TestIfAvailableClient();

            int cFriends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            for (int i = 0; i < cFriends; i++)
            {
                FriendGameInfo_t friendGameInfo;
                CSteamID steamIDFriend = (CSteamID)SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                if (SteamFriends.GetFriendGamePlayed(steamIDFriend, out friendGameInfo) && friendGameInfo.m_steamIDLobby.IsValid())
                {
                    m_friendLobbies[s_nFriendLobbiesFound++] = friendGameInfo.m_steamIDLobby;
                    SteamMatchmaking.RequestLobbyData(friendGameInfo.m_steamIDLobby);
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
                return (CSteamID)SteamMatchmaking.GetLobbyByIndex(Index);
            }
        }

        public static string GetLobbyData(int Index, string Key)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            return SteamMatchmaking.GetLobbyData(steamIDLobby, Key);
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

            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamMatchmaking.JoinLobby(LobbyID);
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
            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamMatchmaking.CreateLobby(type, 4);
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

            SteamMatchmaking.SetLobbyData(s_CurrentLobby.m_handle, Key, Value);
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

            return SteamMatchmaking.GetLobbyData(s_CurrentLobby.m_handle, Key);
        }

        public static int GetLobbyMemberCount(int Index)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            return SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);
        }

        public static int GetLobbyCapacity(int Index)
        {
            CSteamID steamIDLobby = GetLobby(Index);
            return SteamMatchmaking.GetLobbyMemberLimit(steamIDLobby);
        }

        public static bool SetLobbyMemberLimit(int MaxMembers)
        {
            if (SteamCore.InOfflineMode()) return false;
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return false;

            return SteamMatchmaking.SetLobbyMemberLimit(s_CurrentLobby.m_handle, MaxMembers);
        }

        public static void SetLobbyType(int LobbyType)
        {
            if (SteamCore.InOfflineMode()) return;
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            ELobbyType type = IntToLobbyType(LobbyType);
            SteamMatchmaking.SetLobbyType(s_CurrentLobby.m_handle, type);
        }

        public static void SendChatMsg(string Msg)
        {
            if (SteamCore.InOfflineMode())
            {
                s_OnChatMsg.Invoke(Msg, SteamCore.PlayerId(), "player");
                return;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            var bytes = StringHelper.GetBytes(Msg);
            SteamMatchmaking.SendLobbyChatMsg(s_CurrentLobby.m_handle, bytes, bytes.Length);
        }

        public static int GetLobbyMemberCount()
        {
            if (SteamCore.InOfflineMode())
            {
                return 1;
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return -1;
            return SteamMatchmaking.GetNumLobbyMembers(s_CurrentLobby.m_handle);
        }

        public static String GetMemberName(int Index)
        {
            if (SteamCore.InOfflineMode())
            {
                return "Local player";
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return "";

            CSteamID steamIDLobbyMember = (CSteamID)SteamMatchmaking.GetLobbyMemberByIndex(s_CurrentLobby.m_handle, Index);

            return SteamFriends.GetFriendPersonaName(steamIDLobbyMember);
        }

        public static UInt64 GetMemberId(int Index)
        {
            if (SteamCore.InOfflineMode())
            {
                return SteamCore.PlayerId();
            }

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return 0;

            CSteamID steamIDLobbyMember = (CSteamID)SteamMatchmaking.GetLobbyMemberByIndex(s_CurrentLobby.m_handle, Index);

            return (UInt64)steamIDLobbyMember;
        }

        public static bool IsLobbyOwner()
        {
            if (SteamCore.InOfflineMode()) return true;

            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return false;
            return (CSteamID)SteamUser.GetSteamID() == (CSteamID)SteamMatchmaking.GetLobbyOwner(s_CurrentLobby.m_handle);
        }

        public static void LeaveLobby()
        {
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            SteamMatchmaking.LeaveLobby(s_CurrentLobby.m_handle);
            s_CurrentLobby.m_handle.Clear();
        }

        public static void SetLobbyJoinable(bool Joinable)
        {
            //REMOVE//if ( s_CurrentLobby.m_handle == null ) return;

            SteamMatchmaking.SetLobbyJoinable(s_CurrentLobby.m_handle, Joinable);
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
            var bytes = StringHelper.GetBytes(Message);

            SteamNetworking.SendP2PPacket(User, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable, 0);
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

            SteamNetworking.SendP2PPacket(User, pchMsg, len, EP2PSend.k_EP2PSendReliable, 0);
        }

        public static bool MessageAvailable()
        {
            uint msgSize = 0;
            bool result = SteamNetworking.IsP2PPacketAvailable(out msgSize);

            return result;
        }

        public static Tuple<UInt64, String> ReadMessage()
        {
            UInt32 msgSize = 0;
            bool result = SteamNetworking.IsP2PPacketAvailable(out msgSize, 0);

            if (!result)
            {
                return null;
            }

            var packet = new byte[msgSize];
            string msg = "";
            CSteamID steamIDRemote;
            UInt32 bytesRead = 0;

            if (SteamNetworking.ReadP2PPacket(packet, msgSize, out bytesRead, out steamIDRemote, 0))
            {
                msg = packet.ToString();
            }

            return new Tuple<UInt64, String>((ulong)steamIDRemote.GetAccountID(), msg);
        }

        public static Tuple<UInt64, byte[]> ReadBytes()
        {
            UInt32 msgSize = 0;
            bool result = SteamNetworking.IsP2PPacketAvailable(out msgSize, 0);

            if (!result)
            {
                return null;
            }

            var packet = new byte[msgSize];
            CSteamID steamIDRemote;
            UInt32 bytesRead = 0;

            byte[] Bytes = new byte[msgSize];
            if (SteamNetworking.ReadP2PPacket(packet, msgSize, out bytesRead, out steamIDRemote, 0))
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
            SteamNetworking.AcceptP2PSessionWithUser(Player.m_handle);
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
            return SteamFriends.GetFriendPersonaName(m_handle);
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
            return SteamUserStats.RequestCurrentStats();
        }

        public bool GiveAchievement(string AchievementApiName)
        {
            return SteamUserStats.SetAchievement(AchievementApiName);
        }

        public int NumEntries(SteamLeaderboard_t hSteamLeaderboard)
        {
            return SteamUserStats.GetLeaderboardEntryCount(hSteamLeaderboard);
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

            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamUserStats.FindLeaderboard(LeaderboardName);
            SteamMatches.g_CallResultFindLeaderboard = new CallResult<LeaderboardFindResult_t>(CallbackClass.instance.OnFindLeaderboard);
            SteamMatches.g_CallResultFindLeaderboard.Set(hSteamAPICall); //, SteamStats.g_CallbackClassInstance, CallbackClass.OnFindLeaderboard);
        }

        public void UploadScore(LeaderboardHandle Handle, int Value)
        {
            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamUserStats.UploadLeaderboardScore(Handle.m_handle, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, Value, null, 0);
        }

        public void RequestEntries(LeaderboardHandle Handle, int RequestType, int Start, int End, Action<bool> OnDownload)
        {
            s_OnDownload = OnDownload;

            // Request the specified leaderboard data.
            SteamAPICall_t hSteamAPICall = (SteamAPICall_t)SteamUserStats.DownloadLeaderboardEntries(
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
            return SteamFriends.GetFriendPersonaName(m_leaderboardEntries[Index].m_steamIDUser);
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
            uint cchText = SteamUtils.GetEnteredGamepadTextLength();

            string pchText;
            SteamUtils.GetEnteredGamepadTextInput(out pchText, cchText);

            return pchText;
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
                                  //REMOVE//bool val = SteamGameServerUtils_ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, pchDescription, unCharMax, pchInitialText);

                return val;
            }
        }
    }
}

