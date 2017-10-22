﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface IJobSearch
    {

        Task<ResultWithFacetDto<JobDto>> SearchAsync(string term,
            SearchRequestFilter filter,
            SearchRequestSort sort,
            string jobType,
            GeoPoint location,
            CancellationToken token);


        //Task<JobFacetDto> SearchAsync(
        //    string term,
        //    SearchRequestFilter filter,
        //    SearchRequestSort sort,
        //    string jobType,
        //    GeoPoint location,
        //    CancellationToken token);
    }
}