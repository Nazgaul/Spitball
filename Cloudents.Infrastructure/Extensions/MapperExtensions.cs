using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core;

namespace Cloudents.Infrastructure.Extensions
{
    public static class MapperExtensions
    {
        public static IEnumerable<TDestination> MapWithPriority<TSource, TDestination>(this IMapper mapper,
            IEnumerable<TSource> source) where TDestination : IShuffleable
        {
            return source.Select((s, i) =>
            {
                var t = mapper.Map<TSource, TDestination>(s);
                //t.Priority = priority;
                t.Order = i + 1;
                return t;
            });
            //return retVal.Results.Select((s, i) =>
            //{
            //    var t = _mapper.Map<TutorDto>(s.Document);
            //    t.Order = ++i;
            //    return t;
            //});
        }
    }
}