using System;

using SteamWrapper;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        static string DumpedState = "";
        void DumpState(object sender, JavascriptMethodEventArgs e)
        {
            DumpedState = (string)e.Arguments[0];
        }

        void ReturnToLobby(object sender, JavascriptMethodEventArgs e)
        {
            LeaveGameNetwork();

            ResetLobby();

            Send("removeMode", "in-game");
            Send("setMode", "main-menu");

            State = GameState.MainMenu;
            SteamWrapper.SteamHtml.AllowMouseEvents = true;
            
            SetMapThreadFunc(GameMapName);
        }

        void LeaveGame(object sender, JavascriptMethodEventArgs e)
        {
            // Use this if you want leaving a game to return you to the lobby,
            // rather than returning you to the main menu.
            //return ReturnToLobby(sender, e);

            LeaveGameNetwork();

            SteamMatches.LeaveLobby();

            ReturnToMainMenu();
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
            SteamWrapper.SteamHtml.AllowMouseEvents = true;
        }

        void QuitApp(object sender, JavascriptMethodEventArgs e)
        {
            Exit();
        }

        void RequestPause(object sender, JavascriptMethodEventArgs e)
        {
            Networking.ToServer(new Message(MessageType.RequestPause));
        }

        void RequestUnpause(object sender, JavascriptMethodEventArgs e)
        {
            Networking.ToServer(new Message(MessageType.RequestUnpause));
        }
    }
}
