using System;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace Game
{
    public static class BinaryWriterExtension
    {
        public static void Write(this BinaryWriter writer, Texture2D texture)
        {
            var mstream = new MemoryStream();
            Png.ToPng(texture, mstream);
            
            byte[] b = mstream.GetBuffer();
            writer.Write(b.Length);
            writer.Write(b);
        }
    }

    public static class BinaryReaderExtension
    {
        public static Texture2D ReadTexture2D(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[] b = new byte[length];
            reader.Read(b, 0, length);

            var mstream = new MemoryStream(b);
            var texture = Png.FromPng(mstream);
            return texture;
        }
    }

    public static class Png
    {
        public static void ToPng(Texture2D texture, Stream stream)
        {
            int w = texture.Width, h = texture.Height;

            using (Bitmap bitmap = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            {
                byte[] textureData = new byte[4 * w * h];
                texture.GetData<byte>(textureData);

                Rectangle rect = new Rectangle(0, 0, w, h);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                var safePtr = bitmapData.Scan0;

                Marshal.Copy(textureData, 0, safePtr, textureData.Length);

                bitmap.UnlockBits(bitmapData);
                bitmap.Save(stream, ImageFormat.Png);
            }
        }

        public static Texture2D FromPng(Stream stream)
        {
            using (Bitmap bitmap = new Bitmap(stream))
            {
                int w = bitmap.Width, h = bitmap.Height;
                byte[] textureData = new byte[4 * w * h];
                Texture2D texture = new Texture2D(GameClass.Graphics, w, h);

                Rectangle rect = new Rectangle(0, 0, w, h);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var safePtr = bitmapData.Scan0;

                Marshal.Copy(safePtr, textureData, 0, textureData.Length);

                bitmap.UnlockBits(bitmapData);

                texture.SetData(textureData);
                return texture;
            }
        }
    }

    public partial class World : SimShader
    {
        public static byte[] WorldBytes;

        public void SaveInBuffer()
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);

            Save(writer);

            WorldBytes = ms.GetBuffer();

            writer.Close();
            ms.Close();
        }

        public void SaveCurrentStateInBuffer()
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress);
            var writer = new BinaryWriter(zip);

            SaveCurrentState(writer);

            WorldBytes = ms.GetBuffer();

            writer.Close();
            zip.Close();
            ms.Close();
        }

        public void Save(string FileName)
        {
            var stream = new FileStream(FileName, FileMode.Create);
            var writer = new BinaryWriter(stream);

            Save(writer);

            writer.Close();
            stream.Close();
        }

        public void Save(BinaryWriter writer)
        {
            // Grid data
            writer.Write(DataGroup.CurrentData);
            writer.Write(DataGroup.CurrentUnits);
            writer.Write(DataGroup.PreviousData);
            writer.Write(DataGroup.PreviousUnits);
            writer.Write(DataGroup.Extra);
            writer.Write(DataGroup.TargetData);
            writer.Write(DataGroup.Tiles);
            writer.Write(DataGroup.Corpses);
            writer.Write(DataGroup.Magic);
            writer.Write(DataGroup.Necromancy);
            writer.Write(DataGroup.AntiMagic);
            writer.Write(DataGroup.DistanceToOtherTeams);

            writer.Write(DataGroup.RandomField);

            writer.Write(DataGroup.Geo);
            writer.Write(DataGroup.AntiGeo);
            foreach (var dir in Dir.Vals) writer.Write(DataGroup.Dirward[dir]);

            // Info
            writer.Write(CameraPos.x);
            writer.Write(CameraPos.y);
            writer.Write(CameraZoom);
            for (int i = 1; i <= 4; i++) PlayerInfo[i].Write(writer);
        }

        public void SaveCurrentState(BinaryWriter writer)
        {
            // Grid data
            writer.Write(DataGroup.CurrentData);
            writer.Write(DataGroup.CurrentUnits);
            writer.Write(DataGroup.Extra);
            writer.Write(DataGroup.TargetData);
            writer.Write(DataGroup.Corpses);

            // Info
            for (int i = 1; i <= 4; i++) PlayerInfo[i].Write(writer);
        }

        public void LoadFromBuffer()
        {
            Render.UnsetDevice();

            var ms = new MemoryStream(WorldBytes);
            var reader = new BinaryReader(ms);

            Load(reader);

            reader.Close();
            ms.Close();
        }

        public void LoadStateFromBuffer(byte[] bytes)
        {
            Render.UnsetDevice();

            var ms = new MemoryStream(WorldBytes);
            var zip = new GZipStream(ms, CompressionMode.Decompress);
            var reader = new BinaryReader(zip);

            LoadCurrentState(reader);

            reader.Close();
            zip.Close();
            ms.Close();
        }

        public void LoadStateFromBuffer()
        {
            LoadStateFromBuffer(World.WorldBytes);
        }

        public void Reload(byte[] bytes)
        {
            vec2 HoldCamPos = CameraPos;
            float HoldCamZoom = CameraZoom;

            Load(MapFileName);

            RepeatTry(() =>
            {
                LoadStateFromBuffer(bytes);
                GameClass.Data.DoUnitSummary(MyPlayerValue, true);
            });

            CameraPos = HoldCamPos;
            CameraZoom = HoldCamZoom;
        }

        public string Name, MapFileName;
        public void Load(string FileName, int Retries = 10000, bool DataOnly = false)
        {
            MapFileName = FileName;
            Name = Path.GetFileName(FileName);

            RepeatTry(() => _Load(FileName, DataOnly: DataOnly), Retries);
        }

        private void RepeatTry(Action Do, int Retries = 10, int Delay = 100)
        {
            do
            {
                try
                {
                    Do();
                    return;
                }
                catch (IOException e)
                {
                    System.Console.WriteLine(e);
                }

                System.Threading.Thread.Sleep(Delay);
            }
            while (Retries-- > 0);
        }

        void _Load(string FileName, bool DataOnly = false)
        {
            Render.UnsetDevice();

            var stream = new FileStream(FileName, FileMode.Open);
            var reader = new BinaryReader(stream);

            Load(reader);

            reader.Close();
            stream.Close();

            if (!DataOnly)
            {
                Startup();
            }

            //Migrate();
        }

        public bool LoadPlayerInfo = false;
        public void Load(BinaryReader reader)
        {
            // Grid data
            DataGroup.CurrentData.SetData(reader.ReadTexture2D().GetData());
            DataGroup.CurrentUnits.SetData(reader.ReadTexture2D().GetData());
            DataGroup.PreviousData.SetData(reader.ReadTexture2D().GetData());
            DataGroup.PreviousUnits.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Extra.SetData(reader.ReadTexture2D().GetData());
            DataGroup.TargetData.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Tiles.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Corpses.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Magic.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Necromancy.SetData(reader.ReadTexture2D().GetData());
            DataGroup.AntiMagic.SetData(reader.ReadTexture2D().GetData());
            DataGroup.DistanceToOtherTeams.SetData(reader.ReadTexture2D().GetData());

            DataGroup.RandomField.SetData(reader.ReadTexture2D().GetData());

            DataGroup.Geo.SetData(reader.ReadTexture2D().GetData());
            DataGroup.AntiGeo.SetData(reader.ReadTexture2D().GetData());
            foreach (var dir in Dir.Vals) DataGroup.Dirward[dir].SetData(reader.ReadTexture2D().GetData());

            // Info
            CameraPos.x = reader.ReadSingle();
            CameraPos.y = reader.ReadSingle();
            CameraZoom = reader.ReadSingle();

            if (!LoadPlayerInfo) return;
            for (int i = 1; i <= 4; i++) PlayerInfo[i].Read(reader);
        }

        public void LoadCurrentState(BinaryReader reader)
        {
            // Grid data
            DataGroup.CurrentData.SetData(reader.ReadTexture2D().GetData());
            DataGroup.CurrentUnits.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Extra.SetData(reader.ReadTexture2D().GetData());
            DataGroup.TargetData.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Corpses.SetData(reader.ReadTexture2D().GetData());

            // Info
            for (int i = 1; i <= 4; i++) PlayerInfo[i].Read(reader);
        }
    }
}
