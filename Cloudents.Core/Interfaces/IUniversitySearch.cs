using Cloudents.Core.Documents.Queries.GetDocumentsList;
using Cloudents.Core.DTOs;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
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

    public interface IDocumentsSearch
    {
        Task<(IEnumerable<DocumentSearchResultWithScore> result,
            IEnumerable<string> facetSubject)> SearchAsync(DocumentQuery query, UserProfile userProfile, CancellationToken token);
    }
}
