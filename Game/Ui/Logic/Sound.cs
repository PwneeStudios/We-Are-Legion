using Awesomium.Core;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void BindMethods_Sound()
        {
            xnaObj.Bind("PlaySound", PlaySound);
        }

        JSValue PlaySound(object sender, JavascriptMethodEventArgs e)
        {
            if (!awesomium.AllowMouseEvents) return JSValue.Null;

            string soundName = e.Arguments[0].ToString();
            float volume = ArgTo0to1(e.Arguments[1]);

            var sound = SoundWad.Wad.FindByName(soundName);
            if (sound != null) sound.Play(volume);

            return JSValue.Null;
        }
    }
}
