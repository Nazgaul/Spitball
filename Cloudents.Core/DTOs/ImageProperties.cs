using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [Serializable, DataContract]
    public class ImageProperties
    {
        public enum BlurEffect
        {
            None,
            Part,
            All
        }
        public ImageProperties(Uri path, BlurEffect blur)
        {
            Path = path.AbsoluteUri;
            Blur = blur;
        }

        public ImageProperties(Uri path)
        {
            Path = path.AbsoluteUri;
        }

        protected ImageProperties()
        {

        }

        [DataMember(Order = 1)]
        public string Path { get; private set; }
        [DataMember(Order = 2)]
        public BlurEffect? Blur { get; private set; }




        //private const string ImageHashKey = "59b514174bffe4ae402b3d63aad79fe0";


        //public byte[] Encrypt()
        //{
        //    var key = Encoding.ASCII.GetBytes(ImageHashKey.Substring(0, 32));
        //    var ivByte = Encoding.ASCII.GetBytes($"{Path}_{Blur}".PadRight(16, 'a').Substring(0, 16));
        //    using (var aesAlg = Aes.Create())
        //    {
        //        using (var encryptor = aesAlg.CreateEncryptor(key, ivByte))
        //        {
        //            using (var msEncrypt = new MemoryStream())
        //            {
        //                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //                {
        //                    var formatter = new BinaryFormatter();
        //                    formatter.Serialize(csEncrypt, this);
        //                }

        //                var iv = ivByte;

        //                var decryptedContent = msEncrypt.ToArray();

        //                var result = new byte[iv.Length + decryptedContent.Length];

        //                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        //                Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);
        //                return result;
        //                // return Convert.ToBase64String(result);
        //            }
        //        }
        //    }
        //}

        //public static ImageProperties Decrypt(byte[] hash)
        //{
        //    //var fullCipher = Convert.FromBase64String(hash);

        //    var iv = new byte[16];
        //    var cipher = new byte[hash.Length - iv.Length]; //changes here

        //    Buffer.BlockCopy(hash, 0, iv, 0, iv.Length);
        //    // Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
        //    Buffer.BlockCopy(hash, iv.Length, cipher, 0, hash.Length - iv.Length); // changes here
        //    var key = Encoding.UTF8.GetBytes(ImageHashKey.Substring(0, 32));

        //    using (var aesAlg = Aes.Create())
        //    {
        //        using (var decryptor = aesAlg.CreateDecryptor(key, iv))
        //        {
        //            using (var msDecrypt = new MemoryStream(cipher))
        //            {
        //                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //                {
        //                    var formatter = new BinaryFormatter();
        //                    return (ImageProperties)formatter.Deserialize(csDecrypt);
        //                }
        //            }

        //        }
        //    }
        //}
    }
}