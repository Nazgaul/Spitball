using System;
using Zbang.Zbox.Infrastructure.Cache;

namespace Zbang.Zbox.Infrastructure.Url
{
    public class ShortCodesCache : IShortCodesCache
    {
        readonly ICache m_Cache;
        readonly TimeSpan m_CacheHeld = TimeSpan.FromDays(7);

        //public static void Init(IUnityContainer container)
        //{
        //    _container = container;
        //}



        public ShortCodesCache(ICache cache/*, IUnityContainer container*/)
        {
            m_Cache = cache;
            //_container = container;
        }

        public string LongToShortCode(long number, ShortCodesType code = ShortCodesType.Box)
        {
            if (number <= 0)
            {
                throw new ArgumentException("cannot be less or zero from 0", "number");
            }
            var result = ShortCodes.LongToShortCode(number, code);
            return result;
        }

        public long ShortCodeToLong(string shortcode, ShortCodesType code = ShortCodesType.Box)
        {
            if (string.IsNullOrWhiteSpace(shortcode))
            {
                throw new ArgumentException("short code cannot be null or empty", "shortcode");
            }
            var cacheKey = code + shortcode;

           // var cacheResult = m_Cache.GetFromCache(cacheKey, "ShortCode");


            long result;
            //if (cacheResult == null)
            //{
                result = ShortCodes.ShortCodeToLong(shortcode, code);
              //  m_Cache.AddToCache(cacheKey, result, m_CacheHeld, "ShortCode");
            //}
            //else
            //{
            //    try
            //    {
            //        result = (long)cacheResult;
            //    }
            //    catch
            //    {
            //        m_Cache.RemoveFromCache("ShortCode", new System.Collections.Generic.List<string>());
            //        result = ShortCodes.ShortCodeToLong(shortcode, code);
            //    }
            //}

            return result;
        }


        public static IShortCodesCache Create()
        {
            return Ioc.IocFactory.Unity.Resolve<IShortCodesCache>();
        }


    }
}
