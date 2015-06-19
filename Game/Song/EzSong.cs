using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using XnaAudio = Microsoft.Xna.Framework.Audio;
using XnaMedia = Microsoft.Xna.Framework.Media;

namespace Game
{
	public class XnaSong : BaseSong
	{
		public XnaMedia.Song song;

        public override double Play(bool DisplayInfo)
        {
            base.Play(DisplayInfo);

            try
            {
				MediaPlayer.Instance.Stop();
				XnaMedia.MediaPlayer.Play(song);
            }
            catch (Exception e)
            {
                SongWad.Wad.Stop();
                return 1000000;
            }

            return song.Duration.TotalSeconds;
        }

        protected override void LoadSong(string name)
        {
			//song = GameClass.Game.Content.LoadTillSuccess<XnaMedia.Song>("Music\\" + name);
            song = GameClass.Game.Content.Load<XnaMedia.Song>("Music\\" + name);
        }
	}

    public abstract class BaseSong
    {
        public string Name, SongName, ArtistName, FileName;
        public bool Enabled, AlwaysLoaded;

        public float Volume;

        public bool DisplayInfo = true;

        public BaseSong()
        {
            Volume = 1f;
            Enabled = true;
        }

        public virtual double Play(bool DisplayInfo)
        {
            DisplayInfo = Play_SetPlayerParams(DisplayInfo);

            return 0;
        }

        private bool Play_SetPlayerParams(bool DisplayInfo)
        {
            SongWad.CurSongVolume = Volume;

            if (SongWad.Wad.SuppressNextInfoDisplay)
                SongWad.Wad.SuppressNextInfoDisplay = DisplayInfo = false;

            if (DisplayInfo)
                SongWad.Wad.DisplaySongInfo(this);

            return DisplayInfo;
        }

        public bool Loaded = false;
        public virtual void LoadSong_IfNotLoaded(string name)
        {
            if (!Loaded)
            {
                LoadSong(name);
            }

            Loaded = true;
        }

        protected virtual void LoadSong(string name)
        {
        }
    }
}