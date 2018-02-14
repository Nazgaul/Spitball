﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Search.Tutor
{
    public interface ITutorProvider
    {
        Task<IEnumerable<TutorDto>> SearchAsync(string term,
            TutorRequestFilter[] filters, TutorRequestSort sort,
            GeoPoint location,
            int page,
            bool isMobile,
            CancellationToken token);
    }
}
