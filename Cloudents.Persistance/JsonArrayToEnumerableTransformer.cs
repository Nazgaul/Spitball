using Cloudents.Core.Extension;
using Cloudents.Query.Stuff;
using Newtonsoft.Json.Linq;
using NHibernate.Transform;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Persistence
{
    public class JsonArrayToEnumerableTransformer<TEntity> : IJsonArrayToEnumerableTransforme<TEntity>
        where TEntity : class
    {
        private readonly IResultTransformer _baseTransformer;

        public JsonArrayToEnumerableTransformer()
        {
            _baseTransformer = Transformers
                .AliasToBean<TEntity>();
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var notComplexTuple = new List<(string, object)>();
            var complexTuple = new List<(string, int)>();
            var i = 0;
            foreach (var alias in aliases)
            {
                var property = typeof(TEntity).GetProperty(alias);
                if (property.GetRealType() == typeof(IEnumerable<string>))
                {
                    complexTuple.Add((alias, i));
                }
                else
                {
                    notComplexTuple.Add((alias, tuple[i]));
                }
                i++;
            }

            var result = _baseTransformer.TransformTuple(notComplexTuple.Select(s => s.Item2).ToArray(), notComplexTuple.Select(s => s.Item1).ToArray());

            foreach (var complex in complexTuple)
            {
                var val = tuple[complex.Item2];
                var result2 = TransformStringToList(val?.ToString());
                var propertyInfo = result.GetType().GetProperty(complex.Item1);
                if (propertyInfo != null) propertyInfo.SetValue(result, result2);
            }

            return result;
        }

        public IEnumerable<string> TransformStringToList(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var jObject = JArray.Parse(value);
            return jObject.Children().Select(s => (string)s.First).ToList();
        }



        public IList TransformList(IList collection)
        {
            return Transformers.AliasToBean<TEntity>().TransformList(collection);
        }


        public bool Equals(JsonArrayToEnumerableTransformer<TEntity> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other._baseTransformer, _baseTransformer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as JsonArrayToEnumerableTransformer<TEntity>);
        }

        public override int GetHashCode()
        {
            return _baseTransformer.GetHashCode();
        }

    }
}
