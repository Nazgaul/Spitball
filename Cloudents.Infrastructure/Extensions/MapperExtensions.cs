using System;
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
            return MapWithPriority<TSource, TDestination>(mapper, source, _ => { });
        }

        public static IEnumerable<TDestination> MapWithPriority<TSource, TDestination>(this IMapper mapper,
            IEnumerable<TSource> source, Action<IMappingOperationOptions<TSource, TDestination>> opts) where TDestination : IShuffleable
        {
            return source?.Select((s, i) =>
            {
                var t = mapper.Map(s,opts);
                t.Order = i + 1;
                return t;
            });
        }
    }
}