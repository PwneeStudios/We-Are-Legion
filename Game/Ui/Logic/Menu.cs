using System;

using SteamWrapper;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        static string DumpedState = "";
        public void DumpState(string state)
        {
            DumpedState = state;
        }

        public void ReturnToLobby()
        {
            LeaveGame(); return; // FIXME. Returning to lobby is buggy right now. Remove this line after the bug is fixed.

            LeaveGameNetwork();

            ResetLobby();

            Send("removeMode", "in-game");
            Send("setMode", "main-menu");

            State = GameState.MainMenu;
            SteamWrapper.SteamHtml.AllowMouseEvents = true;
            
            SetMapThreadFunc(GameMapName);
        }

        public void LeaveGame()
        {
            // Use this if you want leaving a game to return you to the lobby,
            // rather than returning you to the main menu.
            //return ReturnToLobby(sender, e);

            LeaveGameNetwork();

            SteamMatches.LeaveLobby();

            ReturnToMainMenu();
        }

        public static void LeaveGameNetwork()
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

        public void QuitApp()
        {
            Exit();
        }

        public void RequestPause()
        {
            Networking.ToServer(new Message(MessageType.RequestPause));
        }

        public void RequestUnpause()
        {
            Networking.ToServer(new Message(MessageType.RequestUnpause));
        }
    }
}
