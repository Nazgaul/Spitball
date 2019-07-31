﻿using Cloudents.Core.Documents.Queries.GetDocumentsList;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Core.Questions.Queries.GetQuestionsList;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IUniversitySearch
    {
        [ItemCanBeNull]
        Task<UniversitySearchDto> SearchAsync(string term,
            int page,
            string country,
            CancellationToken token);

    }

    public interface ITutorSearch
    {
        Task<IEnumerable<TutorCardDto>> SearchAsync(TutorListTabSearchQuery query, CancellationToken token);
    }

    public interface IQuestionSearch
    {
        Task<QuestionWithFacetDto> SearchAsync(QuestionsQuery query, CancellationToken token);
    }

    public interface IQuestionsSearch
    {
        Task<(IEnumerable<long> result, IEnumerable<QuestionFilter> facetFilter)> SearchAsync(QuestionsQuery query, CancellationToken token);
    }

    public interface IDocumentsSearch
    {
        Task<(IEnumerable<DocumentSearchResultWithScore> result,
            IEnumerable<string> facetSubject)> SearchAsync(DocumentQuery query, UserProfile userProfile, CancellationToken token);
    }
}
