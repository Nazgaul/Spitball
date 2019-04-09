using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Cloudents.Core.DTOs;
using NHibernate.Transform;

namespace Cloudents.Query.Stuff
{
    public class CustomDeepTransformer<T, TU> : IResultTransformer
        where T : QuestionFeedDto, new()
        where TU : CultureInfo 
    {
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < aliases.Length; i++)
            {
                if (aliases[i] != null && tuple[i] != null)
                {
                    dic.Add(aliases[i], tuple[i]);
                }
            }

            var x = new T { CultureInfo = new CultureInfo(dic["CultureInfo"].ToString()) };
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                SetValues(dic, propertyInfo, x);
            }

            foreach (var propertyInfo in typeof(TU).GetProperties())
            {
                SetValues(dic, propertyInfo, x.CultureInfo);
            }
            return x;
        }

        private static void SetValues(IReadOnlyDictionary<string, object> dic, PropertyInfo propertyInfo, object x)
        {
            if (!dic.TryGetValue(propertyInfo.Name, out var value)) return;
            if (value == null) return;

            if (propertyInfo.PropertyType.IsEnum)
            {
                if (HandleEnum(propertyInfo, propertyInfo.PropertyType, x, value)) return;
            }
            if (propertyInfo.Name.Equals("CultureInfo", StringComparison.InvariantCultureIgnoreCase))
            {
                propertyInfo.SetValue(x, new CultureInfo(value.ToString()));
                return;
            }
            if (value is Guid g && propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(x, g.ToString());
                return;

            }
            var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            if (nullableType != null)
            {
                if (nullableType.IsEnum && HandleEnum(propertyInfo, nullableType, x, value))
                {
                    return;
                }
                var z = Convert.ChangeType(value, nullableType);
                propertyInfo.SetValue(x, z);
                return;
            }
            var y = Convert.ChangeType(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(x, y);
        }

        private static bool HandleEnum(PropertyInfo propertyInfo, Type property, object x, object value)
        {
            if (value is string str)
            {
                var e = Enum.Parse(property, str, true);
                propertyInfo.SetValue(x, e);
                return true;
            }

            if (value is int i)
            {
                var t = Enum.ToObject(property, i);
                propertyInfo.SetValue(x, t);
                return true;
            }

            return false;
        }
        public IList TransformList(IList collection)
        {
            return collection;
        }

    }

    public class DeepTransformer<TEntity> : IResultTransformer
    where TEntity : class
    {
        private readonly char _complexChar;
        private readonly IResultTransformer _transformer = null;
        public DeepTransformer(char complexChar = '.')
        {
            _complexChar = complexChar;
        }
        public DeepTransformer(IResultTransformer transformer, char complexChar = '.')
        {
            _complexChar = complexChar;
            _transformer = transformer;
        }
        
        // rows iterator
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var list = new List<string>(aliases);

            var propertyAliases = new List<string>(list);
            var complexAliases = new List<string>();

            for (var i = 0; i < list.Count; i++)
            {
                var aliase = list[i];
                // Aliase with the '.' represents complex IPersistentEntity chain
                if (aliase.Contains(_complexChar))
                {
                    complexAliases.Add(aliase);
                    propertyAliases[i] = null;
                }
            }

            // be smart use what is already available
            // the standard properties string, valueTypes
            object result;
            if (_transformer == null)
            {
                result = Transformers
                 .AliasToBean<TEntity>()
                 .TransformTuple(tuple, propertyAliases.ToArray());
            }
            else
            {
                result = _transformer
                 .TransformTuple(tuple, propertyAliases.ToArray());
            }
           

            TransformPersistentChain(tuple, complexAliases, result, list);

            return result;
        }

        /// <summary>Iterates the Path Client.Address.City.Code </summary>
        protected virtual void TransformPersistentChain(object[] tuple
              , List<string> complexAliases, object result, List<string> list)
        {
            if (!(result is TEntity entity))
            {
                return;
            }
            foreach (var aliase in complexAliases)
            {
                // the value in a tuple by index of current Aliase
                var index = list.IndexOf(aliase);
                var value = tuple[index];
                if (value == null)
                {
                    continue;
                }

                // split the Path into separated parts
                var parts = aliase.Split(_complexChar);
                var name = parts[0];

                var propertyInfo = entity.GetType()
                    .GetProperty(name, BindingFlags.NonPublic
                                       | BindingFlags.Instance
                                       | BindingFlags.Public);

                if (propertyInfo == null)
                {
                    throw new ArgumentNullException($"propery infor of type {entity.GetType().Name} - name {name}");
                }

                object currentObject = entity;

                var current = 1;
                while (current < parts.Length)
                {
                    name = parts[current];
                    var instance = propertyInfo.GetValue(currentObject);
                    if (instance == null)
                    {
                        instance = Activator.CreateInstance(propertyInfo.PropertyType);
                        propertyInfo.SetValue(currentObject, instance);
                    }

                    propertyInfo = propertyInfo.PropertyType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    currentObject = instance;
                    current++;
                }

                // even dynamic objects could be injected this way
                if (currentObject is IDictionary dictionary)
                {
                    dictionary[name] = value;
                }
                else
                {
                    if (propertyInfo.PropertyType.IsEnum && value is string str)
                    {
                        var e = Enum.Parse(propertyInfo.PropertyType, str, true);
                        propertyInfo.SetValue(currentObject, e);
                    }
                    else
                    {
                        propertyInfo.SetValue(currentObject, Convert.ChangeType(value, propertyInfo.PropertyType));
                    }

                    //propertyInfo.SetValue(currentObject, value);
                }
            }
        }

        // convert to DISTINCT list with populated Fields
        public IList TransformList(IList collection)
        {
            return Transformers.AliasToBean<TEntity>().TransformList(collection);
        }
    }
}