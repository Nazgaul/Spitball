using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Cloudents.Core.DTOs
{
    public class DocumentDetailDto
    {
        private string _name;

        public long Id { get; set; }

        public string Name
        {
            get => Path.GetFileNameWithoutExtension(_name);
            set => _name = value;
        }

        public DateTime Date { get; set; }

        public string University { get; set; }

        public string Course { get; set; }

        public string Professor { get; set; }

        public UserDto User { get; set; }

        public string Type { get; set; }

        public int Pages { get; set; }

        public int Views { get; set; }

        public int Downloads { get; set; }

        public decimal? Price { get; set; }

        public bool IsPurchased { get; set; }

        public int PageCount { get; set; }

    }

    [Serializable]
    public class ImageProperties
    {
        public ImageProperties(long id, int page, bool blur)
        {
            Id = id;
            Page = page;
            Blur = blur;
        }

        public long Id { get; private set; }
        public int Page { get; private set; }
        public bool Blur { get; private set; }


        private const string ImageHashKey = "59b514174bffe4ae402b3d63aad79fe0";


        public byte[] Encrypt()
        {
            var key = Encoding.UTF8.GetBytes(ImageHashKey.Substring(0, 32));
            var ivByte = Encoding.UTF8.GetBytes($"{Id}_{Page}_{Blur}".PadRight(16, 'a').Substring(0, 16));
            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, ivByte))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            var formatter = new BinaryFormatter();
                            formatter.Serialize(csEncrypt, this);
                        }

                        var iv = ivByte;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);
                        return result;
                        // return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static ImageProperties Decrypt(byte[] hash)
        {
            //var fullCipher = Convert.FromBase64String(hash);

            var iv = new byte[16];
            var cipher = new byte[hash.Length - iv.Length]; //changes here

            Buffer.BlockCopy(hash, 0, iv, 0, iv.Length);
            // Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
            Buffer.BlockCopy(hash, iv.Length, cipher, 0, hash.Length - iv.Length); // changes here
            var key = Encoding.UTF8.GetBytes(ImageHashKey.Substring(0, 32));

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            var formatter = new BinaryFormatter();
                            return (ImageProperties)formatter.Deserialize(csDecrypt);
                        }
                    }

                }
            }
        }
    }
}
