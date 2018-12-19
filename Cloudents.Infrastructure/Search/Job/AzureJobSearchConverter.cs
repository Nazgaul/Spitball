using System;
using System.Linq;
using AutoMapper;
using Cloudents.Application.DTOs;
using Cloudents.Infrastructure.Extensions;
using JetBrains.Annotations;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search.Job
{
    [UsedImplicitly]
    internal class AzureJobSearchConverter : ITypeConverter<DocumentSearchResult<Application.Entities.Search.Job>, ResultWithFacetDto<JobProviderDto>>
    {
        public ResultWithFacetDto<JobProviderDto> Convert(DocumentSearchResult<Application.Entities.Search.Job> source, ResultWithFacetDto<JobProviderDto> destination, ResolutionContext context)
        {
            var retVal = new ResultWithFacetDto<JobProviderDto>
            {
                Result = context.Mapper.MapWithPriority<Application.Entities.Search.Job, JobProviderDto>(
                    source.Results.Select(s => s.Document))
            };

            if (source.Facets != null)
            {
                source.Facets.TryGetValue(nameof(Application.Entities.Search.Job.JobType), out var facets);
                retVal.Facet = facets?.Select(s => s.AsValueFacetResult<string>().Value).Where(w => !string.Equals(w,
                    "none", StringComparison.OrdinalIgnoreCase));
            }

            return retVal;
        }
    }
}
