using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Zbang.Zbox.Infrastructure
{
    public static class Md5HashGenerator
    {
        public static string GenerateKey(object sourceObject)
        {
            if (sourceObject == null) throw new ArgumentNullException(nameof(sourceObject));

            var stringObject = sourceObject as string;
            if (stringObject != null)
            {
                return ComputeHash(Encoding.ASCII.GetBytes(stringObject));
            }


            //We determine if the passed object is really serializable.
            try
            {
                //Now we begin to do the real work.
                var hashString = ComputeHash(ObjectToByteArray(sourceObject));
                return hashString;
            }
            catch (AmbiguousMatchException ame)
            {
                throw new ApplicationException("Could not definitely decide if object is serializable.Message:" +
                                               ame.Message);
            }

        }

        private static string ComputeHash(byte[] objectAsBytes)
        {
            using (var md5 = MD5.Create())
            {

                try
                {
                    var hash = md5.ComputeHash(objectAsBytes);
                    var sb = new StringBuilder();
                    foreach (var t in hash)
                    {
                        sb.Append(t.ToString("X2"));
                    }
                    return sb.ToString();
                }
                catch (ArgumentNullException)
                {
                    return null;
                }
            }
        }
        private static readonly object Locker = new object();
        private static byte[] ObjectToByteArray(object objectToSerialize)
        {
            using (var fs = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                try
                {
                    //Here's the core functionality! One Line!
                    //To be thread-safe we lock the object
                    lock (Locker)
                    {
                        formatter.Serialize(fs, objectToSerialize);
                    }
                    return fs.ToArray();
                }
                catch (SerializationException)
                {
                    //Console.WriteLine("Error occurred during serialization. Message: " +
                    //                  se.Message);
                    return null;
                }
            }
        }
    }
}
