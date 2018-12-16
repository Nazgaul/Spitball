﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Common;
using Cloudents.Core.Documents.Queries.GetDocumentsList;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Query;
using Cloudents.Core.Questions.Queries.GetQuestionsList;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IUniversitySearch
    {
        [ItemCanBeNull]
        Task<UniversitySearchDto> SearchAsync(string term,
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

    public interface IQuestionSearch
    {
        Task<QuestionWithFacetDto> SearchAsync(QuestionsQuery query, CancellationToken token);
    }

    public interface IQuestionsSearch
    {
        Task<(IEnumerable<long> result, IEnumerable<QuestionSubject> facetSubject,  IEnumerable<QuestionFilter> facetFileter)> SearchAsync(QuestionsQuery query, CancellationToken token);
    }

    public interface IDocumentsSearch
    {
        Task<IEnumerable<DocumentSearchResultWithScore>> SearchAsync(DocumentQuery query, CancellationToken token);
       // Task<string> ItemMetaContentAsync(long itemId, CancellationToken cancelToken);
    }


}
