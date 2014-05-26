﻿namespace Zbang.Zbox.ViewModel.Queries.User
{
    public class GetDashboardQuery : QueryBase
    {
        public GetDashboardQuery(long userId, long universityId)
            : base(userId)
        {
            UniversityId = universityId;
        }
        public long UniversityId { get; private set; }
    }
}
