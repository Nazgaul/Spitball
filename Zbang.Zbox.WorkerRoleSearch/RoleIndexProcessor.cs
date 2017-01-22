using System;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public static class RoleIndexProcessor
    {
        public  static int GetIndex()
        {
            int currentIndex;

            string instanceId = RoleEnvironment.CurrentRoleInstance.Id;
            bool withSuccess = int.TryParse(instanceId.Substring(instanceId.LastIndexOf(".", StringComparison.Ordinal) + 1), out currentIndex);
            if (!withSuccess)
            {
                int.TryParse(instanceId.Substring(instanceId.LastIndexOf("_", StringComparison.Ordinal) + 1), out currentIndex);
            }
            return currentIndex;
        }

        public static bool IsEmulated { get; } = RoleEnvironment.IsEmulated;
    }

    public enum TimeToSleep
    {
        Min,
        Same,
        Increase
    }
}
