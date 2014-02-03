

namespace Zbang.Zbox.Infrastructure.Ioc
{
    public class IocParameterOverride
    {
        public IocParameterOverride(string parameterName, object parameterValue)
        {
            Name = parameterName;
            Value = parameterValue;
        }
        public string Name { get;private set; }
        public object Value { get; private set; }
    }
}
