﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Infrastructure.Search.Entities;
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

            source.Facets.TryGetValue(context.Items[FacetType].ToString(), out var facets);
            retVal.Facet = facets?.Select(s => s.AsValueFacetResult<string>().Value);

            return retVal;
        }
    }
}
