
namespace Zbang.Zbox.Domain
{
    public class RussianDepartment
    {
// ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual long Id { get; private set; }

        public virtual string Name { get; private set; }
        public virtual string Year { get; private set; }
                
        public virtual University University { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}
