
namespace Zbang.Zbox.Domain
{
    public class RussianDepartment
    {
// ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual long Id { get; protected set; }

        public virtual string Name { get; protected set; }
        public virtual string Year { get; protected set; }

        public virtual University University { get; protected set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}
