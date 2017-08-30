namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class Storage
    {
        public long AllocatedSize { get; set; }
        public long UsedSpace { get; set; }

        public long Percent
        {
            get
            {
                if (AllocatedSize == 0)
                {
                    return 0;
                } 
                return UsedSpace * 100 / AllocatedSize;
            }
        }

        public long Free
        {
            get
            {
                return 100 - Percent;
            }
        }
    }
}