
using System.Web.WebPages;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class DisplayConfig
    {
        internal static void RegisterDisplays()
        {
            DisplayModeProvider.Instance.Modes.Clear();
            DisplayModeProvider.Instance.Modes.Add(new DefaultDisplayMode(string.Empty));
        }
    }
}