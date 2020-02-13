using Cloudents.Core.Extension;
using NHibernate.Transform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cloudents.Query.Stuff
{
    public class DeepTransformer<TEntity> : IResultTransformer
        where TEntity : class
    {
        private readonly char _complexChar;
        private readonly IResultTransformer _baseTransformer;
        public DeepTransformer(char complexChar = '.') : this(complexChar, Transformers.AliasToBean<TEntity>())
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
                var alias = list[i];
                // Aliase with the '.' represents complex IPersistentEntity chain
                if (alias.Contains(_complexChar))
                {
                    complexAliases.Add(alias);
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
            return _baseTransformer.TransformList(collection);
        }


        public bool Equals(DeepTransformer<TEntity> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other._complexChar, _complexChar) && Equals(other._baseTransformer, _baseTransformer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DeepTransformer<TEntity>);
        }

        public override int GetHashCode()
        {
            return _complexChar.GetHashCode() * 23 ^ _baseTransformer.GetHashCode() * 73;
        }
    }
}