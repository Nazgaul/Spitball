
namespace Zbang.Zbox.Infrastructure.Url
{
    public interface IShortCodesCache
    {
        string LongToShortCode(long number, ShortCodesType code = ShortCodesType.Box);
    }
}
