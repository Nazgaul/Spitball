﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Transform;

namespace Cloudents.Query.Stuff
{

 

    public class DeepTransformer<TEntity> : IResultTransformer
    where TEntity : class
    {
        private readonly char _complexChar;
        private readonly IResultTransformer _baseTransformer;
        public DeepTransformer(char complexChar = '.')
        {
            _complexChar = complexChar;
            _baseTransformer = Transformers
                .AliasToBean<TEntity>();

        }

        public DeepTransformer(IResultTransformer transformer, char complexChar = '.')
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