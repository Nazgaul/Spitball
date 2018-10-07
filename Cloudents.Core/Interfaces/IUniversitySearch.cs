﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IUniversitySearch
    {
        [ItemCanBeNull]
        Task<IEnumerable<UniversityDto>> SearchAsync(string term,
            //[CanBeNull]
            //GeoPoint location,
            string country,
            CancellationToken token);

        //[ItemCanBeNull]
        //Task<UniversityDto> GetApproximateUniversitiesAsync(
        //    [NotNull]
        //    GeoPoint location,
        //    CancellationToken token);
    }
}
