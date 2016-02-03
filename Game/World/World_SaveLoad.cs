using System;
using System.IO;
using System.Runtime.InteropServices;

using Hjg.Pngcs;

using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void Test_SaveLoad()
        {
            var fs = new FileStream("C:\\Users\\Jordan\\Desktop\\TestSave.png", FileMode.Create);
            Png.ToPng(Assets.ExplosionTexture_1, fs);
            fs.Close();
        }
    }

    public static class BinaryWriterExtension
    {
        public static void Write(this BinaryWriter writer, Texture2D texture)
        {
            var mstream = new MemoryStream();
            Png.ToPng(texture, mstream);
            
            byte[] b = mstream.ToArray();

            Console.WriteLine("Array size is {0}", b.Length);
            writer.Write(b.Length);
            writer.Write(b);

            mstream.Close();
        }
    }

    public static class BinaryReaderExtension
    {
        public static Texture2D ReadTextureData(this BinaryReader reader, Texture2D texture = null)
        {
            int length = reader.ReadInt32();
            Console.WriteLine("Array read size is {0}", length);
            byte[] b = new byte[length];
            reader.Read(b, 0, length);

            var mstream = new MemoryStream(b);
            Png.FromPng(mstream, texture);
            mstream.Close();

            return texture;
        }
    }

    public static class Png
    {
        public static void ToPng(Texture2D texture, Stream stream)
        {
            int channels = 4;
            int w = texture.Width;
            int h = texture.Height;

            var pngw = new PngWriter(stream, new ImageInfo(w, h, 8, true));
            var bytes = new byte[w * channels];

            var colors = texture.GetData();

            for (int row = 0; row < h; row++)
            {
                int count = 0;
                for (int j = 0; j < w; j++)
                {
                    byte R = (byte)colors[row * w + j].R;
                    byte G = (byte)colors[row * w + j].G;
                    byte B = (byte)colors[row * w + j].B;
                    byte A = (byte)colors[row * w + j].A;

                    bytes[count++] = B;
                    bytes[count++] = G;
                    bytes[count++] = R;
                    bytes[count++] = A;
                }

                pngw.WriteRowByte(bytes, row);
            }

            pngw.End();
        }

        public static Texture2D FromPng(Stream stream, Texture2D texture = null)
        {
            var pngr = new PngReader(stream);

            var ms = new MemoryStream();

            int channels = pngr.ImgInfo.Channels;
            int w = pngr.ImgInfo.Cols;
            int h = pngr.ImgInfo.Rows;
            var bytes = new byte[w * h * channels];

            int count = 0;
            for (int row = 0; row < pngr.ImgInfo.Rows; row++)
            {
                ImageLine l1 = pngr.ReadRowInt(row);           // Format: RGBRGB... or RGBARGBA...
                for (int j = 0; j < pngr.ImgInfo.Cols; j++)
                {
                    byte R = (byte)l1.Scanline[j * channels];
                    byte G = (byte)l1.Scanline[j * channels + 1];
                    byte B = (byte)l1.Scanline[j * channels + 2];
                    byte A = (byte)l1.Scanline[j * channels + 3];

                    bytes[count++] = B;
                    bytes[count++] = G;
                    bytes[count++] = R;
                    bytes[count++] = A;
                }
            }

            ms.Close();
            pngr.End();

            if (texture == null) {
                texture = new Texture2D(GameClass.Graphics, w, h);
            }
            
            texture.SetData(bytes);

            return texture;
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

            WorldBytes = ms.ToArray();

            writer.Close();
            ms.Close();
        }

        public void SaveCurrentStateInBuffer()
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);

            SaveCurrentState(writer);

            var bytes = ms.ToArray();

            writer.Close();
            ms.Close();

            WorldBytes = bytes.Compress();
        }

        public void Save(string path)
        {
            MapFilePath = path;
            GameClass.Game.UpdateEditorJsData();

            var stream = new FileStream(path, FileMode.Create);
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
            lock (DataGroup)
            {
                Render.UnsetDevice();

                var ms = new MemoryStream(WorldBytes);
                var reader = new BinaryReader(ms);

                Load(reader);

                reader.Close();
                ms.Close();
            }
        }

        public void LoadStateFromBuffer(byte[] bytes)
        {
            Render.UnsetDevice();

            var uncompressedBytes = bytes.Explode();

            var ms = new MemoryStream(uncompressedBytes);
            var reader = new BinaryReader(ms);

            LoadCurrentState(reader);

            reader.Close();
            ms.Close();
        }

        public void LoadStateFromBuffer()
        {
            LoadStateFromBuffer(World.WorldBytes);
        }

        public void Reload(int step, byte[] bytes)
        {
            vec2 HoldCamPos = CameraPos;
            float HoldCamZoom = CameraZoom;

            Load(MapFilePath);

            RepeatTry(() =>
            {
                LoadStateFromBuffer(bytes);

                if (MyPlayerNumber > 0)
                {
                    GameClass.Data.DoUnitSummary(MyPlayerValue, true);
                }
            });

            CameraPos = HoldCamPos;
            CameraZoom = HoldCamZoom;

            QueuedActions.Clear();

            SimStep = ServerSimStep = AckSimStep = MinClientSimStep = step;

            SentBookend = false;
            PostUpdateFinished = false;
            PostUpdateStep = 0;

            SecondsSinceLastUpdate = 0;
        }

        public string Name, MapFilePath;
        public void Load(string path, int Retries = 10000, bool DataOnly = false)
        {
            MapFilePath = path;
            Name = Path.GetFileName(path);
            GameClass.Game.UpdateEditorJsData();

            RepeatTry(() => _Load(path, DataOnly: DataOnly), Retries);
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
            lock (DataGroup)
            {
                //Render.UnsetDevice();

                var stream = new FileStream(FileName, FileMode.Open);
                var reader = new BinaryReader(stream);

                Load(reader);

                reader.Close();
                stream.Close();

                /*

                // Test saving and loading.
                // Uncommenting this block of code should result in identical behavior when loading maps.

                Save("C:\\Users\\Jordan\\Desktop\\TestSave.png");

                var _stream = new FileStream("C:\\Users\\Jordan\\Desktop\\TestSave.png", FileMode.Open);
                var _reader = new BinaryReader(_stream);

                Load(_reader);

                _reader.Close();
                _stream.Close();

                */

                if (!DataOnly)
                {
                    Startup();
                }
            }

            //Migrate();
        }

        public bool LoadPlayerInfo = false;
        public void Load(BinaryReader reader)
        {
            // Grid data
            reader.ReadTextureData(DataGroup.CurrentData);
            reader.ReadTextureData(DataGroup.CurrentUnits);
            reader.ReadTextureData(DataGroup.PreviousData);
            reader.ReadTextureData(DataGroup.PreviousUnits);
            reader.ReadTextureData(DataGroup.Extra);
            reader.ReadTextureData(DataGroup.TargetData);
            reader.ReadTextureData(DataGroup.Tiles);
            reader.ReadTextureData(DataGroup.Corpses);
            reader.ReadTextureData(DataGroup.Magic);
            reader.ReadTextureData(DataGroup.Necromancy);
            reader.ReadTextureData(DataGroup.AntiMagic);
            reader.ReadTextureData(DataGroup.DistanceToOtherTeams);

            reader.ReadTextureData(DataGroup.RandomField);

            reader.ReadTextureData(DataGroup.Geo);
            reader.ReadTextureData(DataGroup.AntiGeo);
            foreach (var dir in Dir.Vals) reader.ReadTextureData(DataGroup.Dirward[dir]);

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
            reader.ReadTextureData(DataGroup.CurrentData);
            reader.ReadTextureData(DataGroup.CurrentUnits);
            reader.ReadTextureData(DataGroup.Extra);
            reader.ReadTextureData(DataGroup.TargetData);
            reader.ReadTextureData(DataGroup.Corpses);

            // Info
            for (int i = 1; i <= 4; i++) PlayerInfo[i].Read(reader);
        }
    }
}
