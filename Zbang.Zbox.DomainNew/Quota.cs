
namespace Zbang.Zbox.Domain
{
    public class Quota
    {
        protected const long TenGb = 10737418240;
        protected const long FiveGb = 5368709120;

        public Quota()
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            AllocatedSize = 0;
            UsedSpace = 0;
// ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public void AllocateStorage()
        {
            AllocatedSize = FiveGb;
        }

        public virtual long AllocatedSize { get; set; }
        public virtual long UsedSpace { get; set; }
        public long FreeSpace 
        {
            get
            {
                return AllocatedSize - UsedSpace;
            }
        }
    }    
}
