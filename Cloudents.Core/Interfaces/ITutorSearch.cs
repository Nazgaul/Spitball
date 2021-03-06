﻿using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Query;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ITutorSearch
    {
        Task<ListWithCountDto<TutorCardDto>> SearchAsync(TutorListTabSearchQuery query, CancellationToken token);
    }
}
