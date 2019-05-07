using System;
using System.Reflection;

namespace Cloudents.Core.Test
{
    //public class PrivateObject
    //{
    //    private readonly object _obj;
    //    public PrivateObject(object obj)
    //    {
    //        _obj = obj;
    //    }



    //    public object Invoke(string methodName, BindingFlags bindingFlags, params object[] parameters)
    //    {
    //        var methodInfo = _obj.GetType().GetMethod(methodName, bindingFlags);
    //        //object[] parameters = { "parameters here" };
    //        return methodInfo.Invoke(_obj, parameters);
    //    }

    //    public object Invoke(string methodName, params object[] parameters)
    //    {
    //        MethodInfo methodInfo = _obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
    //        //object[] parameters = { "parameters here" };
    //        return methodInfo.Invoke(_obj, parameters);
    //    }

    //    public object GetFieldOrProperty(string name, BindingFlags bindingFlags)
    //    {
    //        return _obj.GetType().GetProperty(name, bindingFlags).GetValue(_obj);
    //    }
    //}


    public class PrivateType
    {
        private readonly Type _type;
        public PrivateType(Type type)
        {
            _type = type;
        }

        public object InvokeStatic(string methodName, BindingFlags bindingFlags, params object[] parameters)
        {
            MethodInfo methodInfo = _type.GetMethod(methodName, bindingFlags);

            return methodInfo.Invoke(null, parameters);
        }
    }
}