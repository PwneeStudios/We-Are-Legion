namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void BindMethods()
        {
            BindMethods_GeneralInput();
            BindMethods_Sound();

            BindMethods_FindLobby();
            BindMethods_Lobby();
            BindMethods_Menu();
            BindMethods_Options();

            BindMethods_InGame();
            BindMethods_Editor();
        }
    }
}
