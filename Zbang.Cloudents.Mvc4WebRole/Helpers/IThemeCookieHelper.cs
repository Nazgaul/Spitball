using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public interface IThemeCookieHelper
    {
        void InjectCookie(Theme value);
        Theme? ReadCookie();
    }
}