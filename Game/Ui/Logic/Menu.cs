using System;

using Awesomium.Core;

using SteamWrapper;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void BindMethods_Menu()
        {
            xnaObj.Bind("LeaveGame", LeaveGame);
            xnaObj.Bind("ReturnToLobby", ReturnToLobby);
            xnaObj.Bind("QuitApp", QuitApp);
            xnaObj.Bind("DumpState", DumpState);

            xnaObj.Bind("RequestPause", RequestPause);
            xnaObj.Bind("RequestUnpause", RequestUnpause);
        }

        static string DumpedState = "";
        JSValue DumpState(object sender, JavascriptMethodEventArgs e)
        {
            DumpedState = (string)e.Arguments[0];

            return JSValue.Null;
        }

        JSValue ReturnToLobby(object sender, JavascriptMethodEventArgs e)
        {
            LeaveGameNetwork();

            ResetLobby();

            Send("removeMode", "in-game");
            Send("setMode", "main-menu");

            State = GameState.MainMenu;
            if (awesomium != null) awesomium.AllowMouseEvents = true;
            
            SetMapThreadFunc(GameMapName);

            return JSValue.Null;
        }

        JSValue LeaveGame(object sender, JavascriptMethodEventArgs e)
        {
            // Use this if you want leaving a game to return you to the lobby,
            // rather than returning you to the main menu.
            //return ReturnToLobby(sender, e);

            LeaveGameNetwork();

            SteamMatches.LeaveLobby();

            ReturnToMainMenu();

            return JSValue.Null;
        }

        private static void LeaveGameNetwork()
        {
            if (Program.Server)
            {
                Networking.ToClients(new Message(MessageType.ServerLeft));
            }
            else
            {
                Networking.ToServer(new Message(MessageType.LeaveGame));
            }

            Networking.FinalSend();
        }

        public void OnFailedToJoinGame()
        {
            ReturnToMainMenu();

            Console.WriteLine("failed to join game");
            GameClass.Game.Send("setScreen", "failed", new { message = "We Are Legion failed to join the game. Please try again." });
        }

        public void ReturnToMainMenu()
        {
            Send("removeMode", "in-game");
            Send("removeMode", "main-menu");

            Send("setMode", "main-menu");
            Send("setScreen", "game-menu");

            State = GameState.MainMenu;
            if (awesomium != null) awesomium.AllowMouseEvents = true;
        }

        JSValue QuitApp(object sender, JavascriptMethodEventArgs e)
        {
            Exit();

            return JSValue.Null;
        }

        JSValue RequestPause(object sender, JavascriptMethodEventArgs e)
        {
            Networking.ToServer(new Message(MessageType.RequestPause));

            return JSValue.Null;
        }

        JSValue RequestUnpause(object sender, JavascriptMethodEventArgs e)
        {
            Networking.ToServer(new Message(MessageType.RequestUnpause));

            return JSValue.Null;
        }
    }
}
