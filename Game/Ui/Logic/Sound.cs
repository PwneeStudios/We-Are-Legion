namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        public void PlaySound(string soundName, float volume)
        {
            if (!SteamWrapper.SteamHtml.AllowMouseEvents)

            volume = ArgTo0to1(volume);

            var sound = SoundWad.Wad.FindByName(soundName);
            if (sound != null) sound.Play(volume);
        }
    }
}
