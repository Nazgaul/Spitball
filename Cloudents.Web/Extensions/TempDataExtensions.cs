﻿using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

//namespace Cloudents.Web.Extensions
//{
//    public static class TempDataExtensions
//    {
//        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
//        {
//            tempData[key] = Serialize(value);
//        }

//        private static string Serialize<T>(T value)
//        {
//            using (var ms = new MemoryStream())
//            {
//                ProtoBuf.Serializer.Serialize(ms, value);
//                return Convert.ToBase64String(ms.ToArray());
//            }
//        }

//        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
//        {
//            var o = tempData.Peek(key);
//            if (o == null)
//            {
//                return null;
//            }

//            try
//            {
//                var bytes = Convert.FromBase64String((string) o);
//                using (var ms = new MemoryStream(bytes))
//                {
//                    return ProtoBuf.Serializer.Deserialize<T>(ms);
//                }
//            }
//            catch (ProtoBuf.ProtoException)
//            {
//                tempData.Remove(key);
//                return null;
//            }
//        }
//    }
//}
