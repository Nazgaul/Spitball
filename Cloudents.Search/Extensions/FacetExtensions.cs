using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Search.Extensions
{
    public static class FacetExtensions
    {

        public static IEnumerable<T> AsEnumFacetResult<T>(this IEnumerable<FacetResult> facets) where T : Enum, IConvertible
        {
            foreach (var facet in facets)
            {
                var val = (int)facet.AsValueFacetResult<long>().Value;
                //if (!Enum.IsDefined(typeof(QuestionSubject), val)) continue;
                var z = Enum.ToObject(typeof(T), val);
                yield return (T)z;
            }
        }


    }

    public static class TagScoringParameter
    {
        public static ScoringParameter GenerateTagScoringParameter(string name, IEnumerable<string>? input)
        {
            if (input == null)
            {
                return new ScoringParameter(name, new string[] { null });
            }

            var inputList = input.ToList();
            if (!inputList.Any())
            {
                return new ScoringParameter(name, new string[] { null });
            }

            return new ScoringParameter(name, inputList.Select(w => w.ToUpperInvariant()));
        }
        public static ScoringParameter GenerateTagScoringParameter(string name, string? input)
        {
            if (input == null)
            {
                return GenerateTagScoringParameter(name, (IEnumerable<string>)null);
            }
            return GenerateTagScoringParameter(name, new[] { input });
        }

    }
}