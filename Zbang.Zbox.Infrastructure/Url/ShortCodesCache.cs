using System;

namespace Zbang.Zbox.Infrastructure.Url
{
    public class ShortCodesCache : IShortCodesCache
    {

        public string LongToShortCode(long number, ShortCodesType code = ShortCodesType.Box)
        {
            if (number <= 0)
            {
                throw new ArgumentException("cannot be less or zero from 0", nameof(number));
            }
            var result = ShortCodes.LongToShortCode(number, code);
            return result;
        }

        //public long ShortCodeToLong(string shortcode, ShortCodesType code = ShortCodesType.Box)
        //{
        //    if (string.IsNullOrWhiteSpace(shortcode))
        //    {
        //        throw new ArgumentException("short code cannot be null or empty", nameof(shortcode));
        //    }
        //    var result = ShortCodes.ShortCodeToLong(shortcode, code);
        //    return result;
        //}
        
    }
}
