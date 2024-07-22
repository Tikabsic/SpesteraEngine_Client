using System.IO;
using Ionic.Zlib;

public static class ByteCompressor
{
    public static byte[] CompressData(byte[] input)
    {
        using (MemoryStream outputStream = new MemoryStream())
        {
            using (ZlibStream deflateStream = new ZlibStream(outputStream, CompressionMode.Compress, CompressionLevel.Default))
            {
                deflateStream.Write(input, 0, input.Length);
            }

            return outputStream.ToArray();
        }
    }

    public static byte[] DecompressData(byte[] compressedData)
    {
        using (MemoryStream compressedStream = new MemoryStream(compressedData))
        using (MemoryStream decompressedStream = new MemoryStream())
        {
            using (ZlibStream zlibStream = new ZlibStream(compressedStream, CompressionMode.Decompress))
            {
                zlibStream.CopyTo(decompressedStream);
            }

            return decompressedStream.ToArray();
        }
    }
}
