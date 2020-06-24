using Cloudents.Core.DTOs;
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

    //public interface IDocumentsSearch
    //{
    //    Task<(IEnumerable<DocumentSearchResultWithScore> result, IEnumerable<string>? facetSubject)> SearchAsync(
    //        DocumentQuery query, UserProfile userProfile, CancellationToken token);
    //}
}
