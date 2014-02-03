using System.IO;

public static class StreamExtensions
{
    public static byte[] ConvertToByteArray(this Stream input)
    {
        input.Seek(0, SeekOrigin.Begin);
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}
