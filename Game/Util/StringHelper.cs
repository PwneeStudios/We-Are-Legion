using System;
using System.IO;
using System.Text;

namespace Game
{
    public static class StringHelper
    {
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string Compress(this string s)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(bytesToEncode.Compress());
        }

        public static string Explode(this string s)
        {
            byte[] compressedBytes = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(compressedBytes.Explode());
        }

        public static string GetString(byte[] bytes)
        {
            int length = bytes.Length / sizeof(char) + 1;

            char[] chars = new char[length];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }

    public static class ByteHelper
    {
        public static byte[] Compress(this byte[] bytesToEncode)
        {
            using (MemoryStream input = new MemoryStream(bytesToEncode))
            using (MemoryStream output = new MemoryStream())
            {
                using (System.IO.Compression.GZipStream zip = new System.IO.Compression.GZipStream(output, System.IO.Compression.CompressionMode.Compress))
                {
                    input.CopyTo(zip);
                }
                return output.ToArray();
            }
        }

        public static string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] Explode(this byte[] compressedBytes)
        {
            using (MemoryStream input = new MemoryStream(compressedBytes))
            using (MemoryStream output = new MemoryStream())
            {
                using (System.IO.Compression.GZipStream zip = new System.IO.Compression.GZipStream(input, System.IO.Compression.CompressionMode.Decompress))
                {
                    zip.CopyTo(output);
                }
                return output.ToArray();
            }
        }
    }
}