namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchQuizesQuery : SearchQuery
    {
        public SearchQuizesQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50)
            : base(term, userId, universityId, pageNumber, rowsPerPage)
        {
        }

        public override string CacheKey => "quizzes " + GetUniversityId();
    }

    public class SearchFlashcardQuery : SearchQuery
    {
        public SearchFlashcardQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50)
            : base(term, userId, universityId, pageNumber, rowsPerPage)
        {
        }

        public override string CacheKey => "flashcard " + GetUniversityId();
    }
}