namespace Cloudents.Infrastructure.Database.Query
{
    //public class UniversityCoursesSynonymQueryHandler : IQueryHandler<UniversityCoursesSynonymQuery, UniversityCoursesSynonymDto>
    //{
    //    private readonly ReadonlySession _readonlySession;

    //    public UniversityCoursesSynonymQueryHandler(ReadonlySession readonlySession)
    //    {
    //        _readonlySession = readonlySession;
    //    }

    //    private class UniversityResult
    //    {
    //        public UniversityResult(string name, string extraSearch)
    //        {
    //            Name = name;
    //            ExtraSearch = extraSearch;
    //        }

    //        public string Name { get; set; }
    //        [CanBeNull] public string ExtraSearch { get; set; }
    //    }

    //    public async Task<UniversityCoursesSynonymDto> GetAsync(UniversityCoursesSynonymQuery query, CancellationToken token)
    //    {
    //        IFutureValue<UniversityResult> universityFuture = null;
    //        if (query.UniversityId.HasValue)
    //        {
    //            universityFuture = _readonlySession.Session.Query<University>()
    //                .Where(w => w.Id == query.UniversityId)
    //                .Select(s => new UniversityResult(s.Name,s.Extra))
    //                .ToFutureValue();
    //        }
    //        var retVal = new UniversityCoursesSynonymDto();
    //        if (universityFuture != null)
    //        {
    //            var university = await universityFuture.GetValueAsync(token);
    //            var resultUniversity = university.ExtraSearch?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Union(new[] { university.Name });
    //            retVal.University = resultUniversity;
    //        }

    //        retVal.Courses = query.Courses;

    //        return retVal;
    //    }
    //}
}