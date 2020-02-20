using System;
using Cloudents.Core.Extension;
using NHibernate.Transform;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cloudents.Core;

namespace Cloudents.Query.Stuff
{
    public class DeepTransformer<TEntity> : IResultTransformer
        where TEntity : class
    {
        private readonly char _complexChar;
        private readonly IResultTransformer _baseTransformer;
        private readonly Dictionary<string, PropertyInfo> _resultClassProperties;

        public DeepTransformer(char complexChar = '.') :
            this(complexChar, Transformers.AliasToBean<TEntity>())
        {
          
               
        }

        public DeepTransformer(char complexChar, IResultTransformer transformer)
        {
            _baseTransformer = transformer;
            _complexChar = complexChar;
            _resultClassProperties = typeof(TEntity).GetProperties(BindingFlags.NonPublic
                                                                   | BindingFlags.Instance
                                                                   | BindingFlags.Public).ToDictionary(x => x.Name, z => z);
        }

        private Dictionary<string, object> _aliasToTupleMap;

        private void MapProperties(object[] tuple, string[] aliases)
        {
            _aliasToTupleMap = new Dictionary<string, object>();

            for (var i = 0; i < aliases.Length; i++)
            {
                var alias = aliases[i];
                if (alias.Contains(_complexChar))
                {
                    _aliasToTupleMap.Add(alias, tuple[i]);
                    aliases[i] = null;
                }
            }
        }

        // rows iterator
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            if (_aliasToTupleMap is null)
            {
                MapProperties(tuple, aliases);
            }

            // be smart use what is already available
            // the standard properties string, valueTypes
            var result = _baseTransformer.TransformTuple(tuple, aliases);

            TransformPersistentChain(result);

            return result;
        }

        /// <summary>Iterates the Path Client.Address.City.Code </summary>
        protected virtual void TransformPersistentChain(
               object result)
        {
            if (!(result is TEntity entity))
            {
                return;
            }
            foreach (var keyValue in _aliasToTupleMap)
            {
                var value = keyValue.Value;
                if (value == null)
                {
                    continue;
                }
                var aliase = keyValue.Key;
                // split the Path into separated parts
                var parts = aliase.Split(_complexChar);
                var name = parts[0];

                var propertyInfo = _resultClassProperties[name];

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
                    
                    current++;
                    propertyInfo = propertyInfo.PropertyType.GetProperty(name,
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                   
                    currentObject = instance;
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