using System;
using System.Collections.Generic;
using Cloudents.Core.Enum;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.Extensions
{
    public static class FacetExtensions {

        public static IEnumerable<T> AsEnumFacetResult<T>(this IEnumerable<FacetResult> facets) where T : Enum, IConvertible
        {
            foreach (var facet in facets)
            {
                var val = (int)facet.AsValueFacetResult<long>().Value;
                if (Enum.IsDefined(typeof(QuestionSubject), val))
                {
                    var z = Enum.ToObject(typeof(T), val);
                    yield return (T)z;
                }
            }
        }
       

    }
}