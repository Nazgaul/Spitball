using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Transform;

namespace Zbang.Zbox.Infrastructure.Data.Transformers
{
    public class AliasToDerivedClassesTransformer : IResultTransformer
    {
        private readonly Type[] m_ResultClasses;
        private readonly Dictionary<string, AliasToBeanResultTransformer> m_AliasToBean;
        public AliasToDerivedClassesTransformer(params Type[] types)
        {
            m_ResultClasses = types;
            m_AliasToBean = new Dictionary<string, AliasToBeanResultTransformer>();
        }
        public IList TransformList(IList collection)
        {
            return collection;
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {

            var dic = new Dictionary<string, object>();

            for (var i = 0; i < aliases.Length; i++)
            {
                dic.Add(aliases[i], tuple[i]);
            }
            var dtoClassToInvoke = dic["discriminator"] + "Dto";
            var typeToInvoke = m_ResultClasses.FirstOrDefault(w => w.Name == dtoClassToInvoke);
            if (typeToInvoke == null)
            {
                throw new ArgumentException("Could not find dto with the name " + dtoClassToInvoke);
            }
            dic.Remove("discriminator");

            var keysToRemove = dic.Where(kvp => kvp.Value == null)
                        .Select(kvp => kvp.Key)
                        .ToArray();
            foreach (var key in keysToRemove)
            {
                dic.Remove(key);
            }


            AliasToBeanResultTransformer bean;
            if (!m_AliasToBean.TryGetValue(dtoClassToInvoke, out bean))
            {
                bean = new AliasToBeanResultTransformer(typeToInvoke);
                m_AliasToBean.Add(dtoClassToInvoke, bean);
            }
            return bean.TransformTuple(dic.Values.ToArray(), dic.Keys.ToArray());

        }
    }

    //public class ParentChildTransformer<T> : IResultTransformer where T : class
    //{
    //    private readonly AliasToBeanResultTransformer m_AliasToBean;
    //    readonly string m_ChildNode;
    //    readonly string m_AliasParent;
    //    readonly Type m_Type;
    //    public ParentChildTransformer(string childNodeName, string aliasParent)
    //    {
    //        m_ChildNode = childNodeName;
    //        m_AliasParent = aliasParent;
    //        m_Type = typeof(T);

    //        m_AliasToBean = new AliasToBeanResultTransformer(typeof(T));
    //    }
    //    public System.Collections.IList TransformList(System.Collections.IList collection)
    //    {
    //        var x = new List<T>();

    //        var parentProperty = m_Type.GetProperty(m_AliasParent);
    //        var idProperty = m_Type.GetProperty("Id");
    //        var childProperty = m_Type.GetProperty(m_ChildNode);

    //        foreach (var item in collection.Cast<T>())
    //        {
    //            var parentid = parentProperty.GetValue(item, null);
    //            if (parentid == null)
    //            {
    //                x.Add(item);
    //                continue;
    //            }
    //            //var parent = x.FirstOrDefault(w => w.GetType().GetProperty("Id").GetValue(w, null) == parentid);
    //            var parent = x.FirstOrDefault(w => (long)idProperty.GetValue(w, null) == (long)parentid);
    //            if (parent == null)
    //            {
    //                continue;
    //                //throw new ArgumentException("There is a child without a parent");
    //            }
    //            var children = childProperty.GetValue(parent, null) as List<T>;
    //            children.Add(item);
    //            //childProperty.SetValue(parent, item, null);

    //        }
    //        return x;

    //        /* T row = Activator.CreateInstance<T>();
    //    Type rowType = row.GetType();

    //    for (int i = 0; i < reader.FieldCount; i++)
    //    {
    //        PropertyInfo info = rowType.GetProperty(reader.GetName(i));
    //        if (info != null)
    //        {
    //            try
    //            {
    //                if (reader.GetValue(i) != null)
    //                    info.SetValue(row, Convert.ChangeType(reader.GetValue(i), info.PropertyType), null);
    //            }
    //            catch (Exception ex)
    //            {
    //                OnTrace("ProcessReader: Error to set property + " + info.Name + " Type: " + info.PropertyType + " Error: " + ex.Message);
    //                //OnError(ex);
    //            }
    //        }
    //    }*/


    //    }

    //    public object TransformTuple(object[] tuple, string[] aliases)
    //    {
    //        return m_AliasToBean.TransformTuple(tuple, aliases);

    //    }
    //}

}