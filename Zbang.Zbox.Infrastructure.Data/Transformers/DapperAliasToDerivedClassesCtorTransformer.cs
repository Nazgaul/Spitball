using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zbang.Zbox.Infrastructure.Data.Transformers
{
    public class DapperAliasToDerivedClassesCtorTransformer
    {
        protected readonly Type[] ResultClasses;

        public DapperAliasToDerivedClassesCtorTransformer(params Type[] types)
        {
            ResultClasses = types;

        }
        public virtual IList TransformList(IEnumerable<dynamic> collection)
        {
            IList retVal = new List<object>();
            foreach (var item in collection)
            {
                var obj = TransformTuple(item);
                retVal.Add(obj);
            }
            return retVal;
        }

        private object TransformTuple(dynamic row)
        {
            //items.First().Discriminator
            var dic = row as IDictionary<string, object>;
           
            //var dic = new Dictionary<string, object>();

            //for (int i = 0; i < aliases.Length; i++)
            //{
            //    dic.Add(aliases[i].ToLower(), tuple[i]);
            //}
            var dtoClassToInvoke = row.Discriminator + "Dto";
            var typeToInvoke = ResultClasses.FirstOrDefault(w => w.Name == dtoClassToInvoke);
            if (typeToInvoke == null)
            {
                throw new ArgumentException("Could not find dto with the name " + dtoClassToInvoke);
            }

            //dic.Remove("discriminator");

            var ctor = typeToInvoke.GetConstructors()[0];
            var itemsToInvoke = new List<object>();

            var ctorParams = ctor.GetParameters();

            //var x = row as KeyValuePair<string, object>;
            foreach (var parameter in ctorParams)
            {
                if (dic == null) continue;
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
