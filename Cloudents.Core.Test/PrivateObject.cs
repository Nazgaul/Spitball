using System;
using System.Reflection;

namespace Cloudents.Core.Test
{
    public class PrivateType
    {
        private readonly Type _type;
        public PrivateType(Type type)
        {
            _type = type;
        }

        public object InvokeStatic(string methodName, BindingFlags bindingFlags, params object[] parameters)
        {
            var methodInfo = _type.GetMethod(methodName, bindingFlags);

            return methodInfo?.Invoke(null, parameters);
        }
    }
}