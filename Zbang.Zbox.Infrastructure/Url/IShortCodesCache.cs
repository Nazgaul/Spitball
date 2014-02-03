
namespace Zbang.Zbox.Infrastructure.Url
{
    public interface IShortCodesCache
    {
        string LongToShortCode(long number, ShortCodesType code = ShortCodesType.Box);
        long ShortCodeToLong(string shortcode, ShortCodesType code = ShortCodesType.Box);
        //IShortCodesCache Create();
    }
}
