using System;
using System.IO;

using Windows = System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

using Awesomium.Core;
using Awesomium.Core.Data;
using Awesomium.Core.Dynamic;
using AwesomiumXNA;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;

using Newtonsoft.Json;

namespace Game
{
    using Dict = Dictionary<string, object>;

    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void BindMethods()
        {
            xnaObj.Bind("OnMouseOver", OnMouseOver);
            xnaObj.Bind("OnMouseLeave", OnMouseLeave);
            xnaObj.Bind("EnableGameInput", EnableGameInput);
            xnaObj.Bind("DisableGameInput", DisableGameInput);
            xnaObj.Bind("DrawMapPreviewAt", DrawMapPreviewAt);
            xnaObj.Bind("HideMapPreview", HideMapPreview);
            xnaObj.Bind("SetMap", SetMap);
            xnaObj.Bind("ActionButtonPressed", ActionButtonPressed);
            xnaObj.Bind("StartGame", StartGame);
            xnaObj.Bind("LeaveGame", LeaveGame);
            xnaObj.Bind("QuitApp", QuitApp);
            xnaObj.Bind("OnChatEnter", OnChatEnter);
            xnaObj.Bind("SetMusicVolume", SetMusicVolume);
            xnaObj.Bind("GetMusicVolume", GetMusicVolume);
            xnaObj.Bind("SetSoundVolume", SetSoundVolume);
            xnaObj.Bind("GetSoundVolume", GetSoundVolume);
            xnaObj.Bind("SetFullscreen", SetFullscreen);
            xnaObj.Bind("GetFullscreen", GetFullscreen);
            xnaObj.Bind("GetFullscreenValues", GetFullscreenValues);
            xnaObj.Bind("SetResolution", SetResolution);
            xnaObj.Bind("GetResolution", GetResolution);
            xnaObj.Bind("GetResolutionValues", GetResolutionValues);
        }

        public bool ShowChat = false;
        public void ToggleChat(Toggle value = Toggle.Flip)
        {
            value.Apply(ref ShowChat);
            UpdateShow();
        }

        public bool ShowAllPlayers = false;
        public void ToggleAllPlayers(Toggle value = Toggle.Flip)
        {
            value.Apply(ref ShowAllPlayers);
            UpdateShow();
        }

        void SendDict(string function, Dictionary<string, object> dict)
        {
            var json = Jsonify(dict);
            Send(function, json);
        }

        void SendString(string function, string s)
        {
            Send(function, '\'' + s + '\'');
        }

        void Send(string function, string s)
        {
            try
            {
                awesomium.WebView.ExecuteJavascript(function + "(" + s + ");");
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not communicate with Awesomium, {0}({1}): {2}", function, s, e);
            }
        }

        void UpdateJsData()
        {
            obj["UnitCount"] = World.DataGroup.UnitCountUi;
            obj["MyPlayerInfo"] = World.MyPlayerInfo;
            obj["MyPlayerNumber"] = World.MyPlayerNumber;
            obj["PlayerInfo"] = ShowAllPlayers ? World.PlayerInfo : null;

            var json = Jsonify(obj);
            SendDict("update", obj);
        }

        void UpdateParams()
        {
            var obj = new Dictionary<string, object>();
            obj["Spells"] = Spells.SpellDict;
            obj["Buildings"] = World.MyPlayerInfo.Params.Buildings;

            SendDict("setParams", obj);
        }

        void UpdateShow()
        {
            var obj = new Dictionary<string, object>();
            obj["ShowChat"] = ShowChat;
            obj["ShowAllPlayers"] = ShowAllPlayers;

            SendDict("show", obj);
        }

        bool _MapLoading = false;
        void UpdateLobbyMapLoading()
        {
            if (_MapLoading != MapLoading)
            {
                _MapLoading = MapLoading;
            }

            var obj = new Dictionary<string, object>();
            obj["LobbyMapLoading"] = MapLoading;

            SendDict("lobby", obj);
        }

        public void AddChatMessage(int player, string message)
        {
            var obj = new Dictionary<string, object>();
            obj["message"] = message;
            obj["player"] = player;
            obj["name"] = PlayerInfo[player].Name;

            SendDict("addChatMessage", obj);
        }

        public bool GameInputEnabled = true;
        JSValue DisableGameInput(object sender, JavascriptMethodEventArgs e)
        {
            GameInputEnabled = false;
            return JSValue.Null;
        }

        JSValue EnableGameInput(object sender, JavascriptMethodEventArgs e)
        {
            GameInputEnabled = true;
            return JSValue.Null;
        }

        public bool DrawMapPreview = false;
        public vec2 MapPreviewPos = vec2.Zero;
        public vec2 MapPreviewSize = vec2.Zero;
        JSValue DrawMapPreviewAt(object sender, JavascriptMethodEventArgs e)
        {
            float x = float.Parse(e.Arguments[0].ToString());
            float y = float.Parse(e.Arguments[1].ToString());
            MapPreviewPos = new vec2(x, y);

            float width = float.Parse(e.Arguments[2].ToString());
            float height = float.Parse(e.Arguments[3].ToString());
            MapPreviewSize = new vec2(width, height);

            DrawMapPreview = true;

            return JSValue.Null;
        }

        JSValue HideMapPreview(object sender, JavascriptMethodEventArgs e)
        {
            DrawMapPreview = false;
            return JSValue.Null;
        }

        Thread SetMapThread, PrevMapThread;
        bool MapLoading = false;
        string GameMapName = null;
        JSValue SetMap(object sender, JavascriptMethodEventArgs e)
        {
            string new_map = e.Arguments[0] + ".m3n";

            if ((SetMapThread == null || !SetMapThread.IsAlive) && GameMapName == new_map) return JSValue.Null;

            GameMapName = new_map;

            PrevMapThread = SetMapThread;
            SetMapThread = new Thread(() => SetMap(GameMapName));
            SetMapThread.Priority = ThreadPriority.Highest;
            SetMapThread.Start();

            return JSValue.Null;
        }

        World NewMap;
        void SetMap(string map)
        {
            if (PrevMapThread != null && PrevMapThread.IsAlive) PrevMapThread.Join();

            if (NewMap != null && NewMap.Name == map) return;
            if (map != GameMapName) return;

            NewMap = null;
            World _NewMap = new World();
            
            MapLoading = true;

            try
            {
                _NewMap.Load(Path.Combine("Content", Path.Combine("Maps", map)), Retries: 0, DataOnly: true);
            }
            catch
            {
                _NewMap.Load(Path.Combine("Content", Path.Combine("Maps", "Beset.m3n")), Retries: 0, DataOnly: true);
            }

            NewMap = _NewMap;
        }

        public bool MouseOverHud = false;
        JSValue OnMouseLeave(object sender, JavascriptMethodEventArgs e)
        {
            MouseOverHud = false;
            //Console.WriteLine(MouseOverHud);
            return JSValue.Null;
        }

        JSValue OnMouseOver(object sender, JavascriptMethodEventArgs e)
        {
            MouseOverHud = true;
            //Console.WriteLine(MouseOverHud);
            return JSValue.Null;
        }

        JSValue ActionButtonPressed(object sender, JavascriptMethodEventArgs e)
        {
            try
            {
                string action = (string)e.Arguments[0];
                Console.WriteLine(action);

                try
                {
                    World.Start(action);
                }
                catch
                {
                    Console.WriteLine("Unrecognized action {0}", action);
                }
            }
            catch
            {
                Console.WriteLine("Action did not specify a name:string.");
            }

            return JSValue.Null;
        }

        JSValue StartGame(object sender, JavascriptMethodEventArgs e)
        {
            //Program.ParseOptions("--client --ip 127.0.0.1 --port 13000 --p 1 --t 1234 --n 2 --map Beset.m3n   --debug --double");
            Program.ParseOptions("--server                --port 13000 --p 1 --t 1234 --n 1 --map Beset.m3n   --debug");
            SetScenarioToLoad("Beset.m3n");
            Networking.Start();

            return JSValue.Null;
        }

        JSValue LeaveGame(object sender, JavascriptMethodEventArgs e)
        {
            SendString("removeMode", "in-game");
            SendString("removeMode", "main-menu");

            SendString("setMode", "main-menu");
            SendString("setScreen", "game-menu");

            State = GameState.MainMenu;
            awesomium.AllowMouseEvents = true;

            return JSValue.Null;
        }

        JSValue QuitApp(object sender, JavascriptMethodEventArgs e)
        {
            Exit();

            return JSValue.Null;
        }

        JSValue OnChatEnter(object sender, JavascriptMethodEventArgs e)
        {
            string message = e.Arguments[0];

            if (message != null && message.Length > 0)
            {
                Console.WriteLine("ui chat message: " + message);
                Networking.ToServer(new MessageChat(message));
            }

            ToggleChat(Toggle.Off);

            return JSValue.Null;
        }

        double MusicVolume, SoundVolume;
        JSValue SetSoundVolume(object sender, JavascriptMethodEventArgs e)
        {
            SoundVolume = double.Parse(e.Arguments[0].ToString());

            return JSValue.Null;
        }

        JSValue GetSoundVolume(object sender, JavascriptMethodEventArgs e)
        {
            return SoundVolume;
        }

        JSValue SetMusicVolume(object sender, JavascriptMethodEventArgs e)
        {
            MusicVolume = double.Parse(e.Arguments[0].ToString());

            return JSValue.Null;
        }

        JSValue GetMusicVolume(object sender, JavascriptMethodEventArgs e)
        {
            return MusicVolume;
        }

        bool Fullscreen;
        JSValue SetFullscreen(object sender, JavascriptMethodEventArgs e)
        {
            Fullscreen = (bool)e.Arguments[0];

            return JSValue.Null;
        }

        JSValue GetFullscreen(object sender, JavascriptMethodEventArgs e)
        {
            return Fullscreen;
        }

        JSValue GetFullscreenValues(object sender, JavascriptMethodEventArgs e)
        {
            var options = new List<Dict>();
            var dict = new Dict();

            dict = new Dict();
            dict["name"] = "Fullscreen";
            dict["value"] = true;
            options.Add(dict);

            dict = new Dict();
            dict["name"] = "Windowed";
            dict["value"] = false;
            options.Add(dict);

            return Jsonify(options);
        }

        int Resolution = 1;
        JSValue SetResolution(object sender, JavascriptMethodEventArgs e)
        {
            Resolution = (int)e.Arguments[0];

            return JSValue.Null;
        }

        JSValue GetResolution(object sender, JavascriptMethodEventArgs e)
        {
            return Resolution;
        }

        JSValue GetResolutionValues(object sender, JavascriptMethodEventArgs e)
        {
            var options = new List<Dict>();
            var dict = new Dict();

            dict = new Dict();
            dict["name"] = "800x600";
            dict["value"] = 1;
            options.Add(dict);

            dict = new Dict();
            dict["name"] = "1280x720";
            dict["value"] = 2;
            options.Add(dict);

            return Jsonify(options);
        }
    }
}
