using NHibernate.Transform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cloudents.Core.Extension;

namespace Cloudents.Query.Stuff
{

    public class SbAliasToBeanResultTransformer<T> : IResultTransformer
    {
        private readonly Type _resultClass;

        public SbAliasToBeanResultTransformer()
        {
            _resultClass = typeof(T);
            
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var retVal = Activator.CreateInstance(_resultClass);
           
            for (int i = 0; i < aliases.Length; i++)
            {
                SetValues(aliases[i], tuple[i], retVal);
            }

            return retVal;
        }

        public IList TransformList(IList collection)
        {
            return collection;
        }


        private  void SetValues(string alias, object value, object x)
        {
            //if (!dic.TryGetValue(propertyInfo.Name, out var value)) return;

            if (value == null) return;
            var propertyInfo = _resultClass.GetProperties().FirstOrDefault(w => string.Equals(w.Name, alias, StringComparison.OrdinalIgnoreCase));
            if (propertyInfo == null)
            {
                return;
            }
            if (propertyInfo.PropertyType.IsEnum)
            {
                if (HandleEnum(propertyInfo, propertyInfo.PropertyType, x, value)) return;
            }
            if (value is Guid g && propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(x, g.ToString());
                return;

            }

            if (HandleTicksTimeSpan(value, x, propertyInfo)) return;
            var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            if (nullableType != null)
            {
                if (nullableType.IsEnum && HandleEnum(propertyInfo, nullableType, x, value))
                {
                    return;
                }
                if (HandleTicksTimeSpan(value, x, propertyInfo)) return;
                var z = Convert.ChangeType(value, nullableType);
                propertyInfo.SetValue(x, z);
                return;
            }
            var y = Convert.ChangeType(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(x, y);
        }

        private static bool HandleTicksTimeSpan(object value, object x, PropertyInfo propertyInfo)
        {
            var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
            if (type == typeof(TimeSpan)&& value is long l  )
            {
                propertyInfo.SetValue(x, new TimeSpan(l).StripMilliseconds());
                return true;
            }

            return false;
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
    }

    public class DeepTransformer<TEntity> : IResultTransformer
        where TEntity : class
    {
        private readonly char _complexChar;
        private readonly IResultTransformer _baseTransformer;
        public DeepTransformer(char complexChar = '.') :this(complexChar, Transformers.AliasToBean<TEntity>())
        {

        }

        public DeepTransformer(char complexChar, IResultTransformer transformer)
        {
            _baseTransformer = transformer;
            _complexChar = complexChar;
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
            var result = _baseTransformer.TransformTuple(tuple, propertyAliases.ToArray());

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
                    propertyInfo.SetValueExtension(currentObject, value);
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