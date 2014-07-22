using System.Collections;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.Infrastructure.Data.Transformers
{
    public class AliasToDerivedClassesCtorTransformer : IResultTransformer
    {
        private readonly Type[] m_ResultClasses;

        public AliasToDerivedClassesCtorTransformer(params Type[] types)
        {
            m_ResultClasses = types;

        }
        public virtual IList TransformList(IList collection)
        {
            foreach (var resultClass in m_ResultClasses)
            {
                if (
                    !resultClass.GetInterfaces()
                        .Any(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof (IComposite<>))) continue;
                var typeToConstruct = typeof(AliasToCompositeClassTransformer<>);
                var constructed = typeToConstruct.MakeGenericType(resultClass);
                var o = (IResultTransformer)Activator.CreateInstance(constructed);
                collection = o.TransformList(collection);
            }
            return collection;
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            if (tuple == null) throw new ArgumentNullException("tuple");
            if (aliases == null) throw new ArgumentNullException("aliases");
            var dic = new Dictionary<string, object>();

            for (var i = 0; i < aliases.Length; i++)
            {
                dic.Add(aliases[i].ToLower(), tuple[i]);
            }
            var dtoClassToInvoke = dic["discriminator"] + "Dto";
            var typeToInvoke = m_ResultClasses.FirstOrDefault(w => w.Name == dtoClassToInvoke);
            if (typeToInvoke == null)
            {
                throw new ArgumentException("Could not find dto with the name " + dtoClassToInvoke);
            }
            dic.Remove("discriminator");

            var ctor = typeToInvoke.GetConstructors()[0];
            var itemsToInvoke = new List<object>();

            var ctorParams = ctor.GetParameters();


            foreach (var parameter in ctorParams)
            {

                var value = dic[parameter.Name.ToLower()];
                var t = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;

                var safeValue = (value == null) ? null
                                                   : Convert.ChangeType(value, t);

                itemsToInvoke.Add(safeValue);

            }

            return ctor.Invoke(itemsToInvoke.ToArray());

        }
    }
}
