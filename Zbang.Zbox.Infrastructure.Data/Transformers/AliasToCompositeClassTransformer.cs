using NHibernate.Transform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.Infrastructure.Data.Transformers
{
    public class AliasToCompositeClassTransformer<T> : IResultTransformer where T : class, IComposite<T>
    {
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var dic = new Dictionary<string, object>();
            for (int i = 0; i < aliases.Length; i++)
            {
                dic.Add(aliases[i].ToLower(), tuple[i]);
            }
            var ctor = typeof(T).GetConstructors()[0];
            var itemsToInvoke = new List<object>();
            var ctorParams = ctor.GetParameters();

            foreach (var parameter in ctorParams)
            {
                var value = dic[parameter.Name.ToLower()];
                Type t = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                itemsToInvoke.Add(safeValue);
            }
            return ctor.Invoke(itemsToInvoke.ToArray());
        }

        public IList TransformList(IList collection)
        {
            IList parents2 = (IList)Activator.CreateInstance(collection.GetType());
            var parentsDictionary = new Dictionary<long, T>();

            var sonsDictionary = new Dictionary<long, T>();
            var sons = new List<T>();
            foreach (var item in collection)
            {
                T elem = item as T;
                if (elem == null)
                {
                    parents2.Add(item);
                    continue;
                }
                if (elem.ParentId != null)
                {
                    T directParent;
                    if (parentsDictionary.TryGetValue(elem.ParentId.Value, out directParent))
                    {
                        directParent.Replies.Add(elem);
                    }
                    else
                    {
                        sons.Add(elem);
                    }
                }
                else
                {
                    parentsDictionary.Add(elem.Id, elem);
                    var directSons = sons.Where(w => w.ParentId == elem.Id);
                    elem.Replies.AddRange(directSons);
                    sons.RemoveAll(s => s.ParentId == elem.Id);
                    parents2.Add(elem);
                }
            }

           
            foreach (var son in sons)
            {
                T value;
                if (parentsDictionary.TryGetValue(son.ParentId.Value, out value))
                {
                    value.Replies.Add(son);
                }

            }
            return parents2;//.ToList();
        }
    }
}
