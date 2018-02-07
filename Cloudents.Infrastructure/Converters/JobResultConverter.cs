using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Converters
{
    internal class JobResultConverter : ITypeConverter<DocumentSearchResult<Job>, ResultWithFacetDto<JobDto>>
    {
        internal const string FacetType = "facet";
        public ResultWithFacetDto<JobDto> Convert(DocumentSearchResult<Job> source, ResultWithFacetDto<JobDto> destination, ResolutionContext context)
        {
            var retVal = new ResultWithFacetDto<JobDto>
            {
                Result = context.Mapper.Map<IEnumerable<Job>, IList<JobDto>>(source.Results.Select(s => s.Document))
            };

            if (source.Facets != null)
            {
                source.Facets.TryGetValue(nameof(Job.JobType), out var facets);
                retVal.Facet = facets?.Select(s => s.AsValueFacetResult<string>().Value).Where(w => !string.Equals(w,
                    "none", StringComparison.InvariantCultureIgnoreCase));
            }

            return retVal;
        }
    }
}
