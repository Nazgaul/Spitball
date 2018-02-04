using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Queries.Jared
{
    public class JaredFavoritesQuery
    {
        public JaredFavoritesQuery(IEnumerable<long> documentIds, IEnumerable<long> quizIds, IEnumerable<long> flashcardIds, IEnumerable<Guid> questionIds)
        {
            DocumentIds = documentIds;
            QuizIds = quizIds;
            FlashcardIds = flashcardIds;
            CommentIds = questionIds;
        }

        public IEnumerable<long> DocumentIds { get; private set; }

        public IEnumerable<long> QuizIds { get; private set; }

        public IEnumerable<long> FlashcardIds { get; private set; }

        public IEnumerable<Guid> CommentIds { get; private set; }
    }
}
