using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace Terracotta
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

    public partial class World
    {
        public void Save(string FileName)
        {
            var stream = new FileStream(FileName, FileMode.Create);
            var writer = new BinaryWriter(stream);

            // Grid data
            writer.Write(DataGroup.CurrentData);
            writer.Write(DataGroup.CurrentUnits);
            writer.Write(DataGroup.PreviousData);
            writer.Write(DataGroup.PreviousUnits);
            writer.Write(DataGroup.Extra);
            writer.Write(DataGroup.TargetData);
            writer.Write(DataGroup.Tiles);
            writer.Write(DataGroup.Corspes);
            //writer.Write(DataGroup.Magic);
            writer.Write(DataGroup.DistanceToOtherTeams);

            writer.Write(DataGroup.RandomField);

            writer.Write(DataGroup.Geo);
            writer.Write(DataGroup.AntiGeo);
            foreach (var dir in Dir.Vals) writer.Write(DataGroup.Dirward[dir]);

            // Info
            writer.Write(CameraPos.x);
            writer.Write(CameraPos.y);
            writer.Write(CameraZoom);
            for (int i = 1; i <= 4; i++)
            {
                writer.Write(PlayerInfo[i].Gold);
                writer.Write(PlayerInfo[i].GoldMines);
            }

            writer.Close();
            stream.Close();
        }

        public void Load(string FileName)
        {
            Render.UnsetDevice();

            var stream = new FileStream(FileName, FileMode.Open);
            var reader = new BinaryReader(stream);

            // Grid data
            DataGroup.CurrentData.SetData(reader.ReadTexture2D().GetData());
            DataGroup.CurrentUnits.SetData(reader.ReadTexture2D().GetData());
            DataGroup.PreviousData.SetData(reader.ReadTexture2D().GetData());
            DataGroup.PreviousUnits.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Extra.SetData(reader.ReadTexture2D().GetData());
            DataGroup.TargetData.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Tiles.SetData(reader.ReadTexture2D().GetData());
            DataGroup.Corspes.SetData(reader.ReadTexture2D().GetData());
            //DataGroup.Magic.SetData(reader.ReadTexture2D().GetData());
            DataGroup.DistanceToOtherTeams.SetData(reader.ReadTexture2D().GetData());

            DataGroup.RandomField.SetData(reader.ReadTexture2D().GetData());

            DataGroup.Geo.SetData(reader.ReadTexture2D().GetData());
            DataGroup.AntiGeo.SetData(reader.ReadTexture2D().GetData());
            foreach (var dir in Dir.Vals) DataGroup.Dirward[dir].SetData(reader.ReadTexture2D().GetData());

            // Info
            CameraPos.x = reader.ReadSingle();
            CameraPos.y = reader.ReadSingle();
            CameraZoom = reader.ReadSingle();
            for (int i = 1; i <= 4; i++)
            {
                PlayerInfo[i].Gold = reader.ReadInt32();
                PlayerInfo[i].GoldMines = reader.ReadInt32();
            }

            reader.Close();
            stream.Close();

            //Render.UnsetDevice();
            //Migrate();
            //Render.UnsetDevice();
        }
    }
}
