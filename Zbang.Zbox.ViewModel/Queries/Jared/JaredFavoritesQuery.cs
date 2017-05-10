using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Queries.Jared
{
    public class JaredFavoritesQuery
    {
        public JaredFavoritesQuery(IEnumerable<long> documentIds, IEnumerable<long> quizIds)
        {
            DocumentIds = documentIds;
            QuizIds = quizIds;
        }

        public IEnumerable<long> DocumentIds { get; private set; }

        public IEnumerable<long> QuizIds { get; private set; }
    }
}
