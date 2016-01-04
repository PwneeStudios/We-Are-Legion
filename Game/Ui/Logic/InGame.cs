using System;
using System.Collections.Generic;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        public void UpdateJsData()
        {
            obj["UnitCount"] = World.DataGroup.UnitCountUi;
            obj["MyPlayerInfo"] = World.MyPlayerInfo;
            obj["MyPlayerNumber"] = World.MyPlayerNumber;
            obj["PlayerInfo"] = ShowAllPlayers ? World.PlayerInfo : null;

            var json = Jsonify(obj);
            Send("update", obj);
        }

        public void UpdateParams()
        {
            if (World.MyPlayerInfo == null || World.MapEditor) return;

            World.MyPlayerInfo.Update();

            var obj = new Dictionary<string, object>();
            obj["SpellCosts"] = World.MyPlayerInfo.SpellCosts;
            obj["Buildings"] = World.MyPlayerInfo.Params.Buildings;

            Send("setParams", obj);
        }

        public void UpdateShow()
        {
            Send("show", new
            {
                ShowChat = ShowChat,
                ShowAllPlayers = ShowAllPlayers,
                ChatGlobal = ChatGlobal,
            });
        }

        public bool ShowChat = false;
        public bool ChatGlobal = true;
        public void ToggleChatViaFlag(Toggle value = Toggle.Flip)
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

        public void AddChatMessage(string name, string message)
        {
            var obj = new Dictionary<string, object>();
            obj["message"] = message;
            obj["name"] = name;

            Send("addChatMessage", obj);
        }

        public void ActionButtonPressed(string action)
        {
            try
            {
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
        }

        public void OnChatEnter(string message)
        {
            if (message != null && message.Length > 0)
            {
                Console.WriteLine("ui chat message: " + message);
                Networking.ToServer(new MessageChat(ChatGlobal, Game.PlayerName(), message));
            }

            ToggleChatViaFlag(Toggle.Off);
        }
    }
}
