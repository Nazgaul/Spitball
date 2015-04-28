using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class FilterProvider : ISearchFilterProvider
    {
        private readonly IUniversityWithCode m_UniversityWithCodeProvider;
        private List<long> m_UniversityWithcode;
        public FilterProvider(IUniversityWithCode universityWithCode)
        {
            m_UniversityWithCodeProvider = universityWithCode;
        }

        public async Task<string> BuildFilterExpression(long universityId, string uniIdField, string userField, long userId)
        {
            if (m_UniversityWithcode == null)
            {
                var retVal = await m_UniversityWithCodeProvider.GetUniversityWithCode();
                m_UniversityWithcode = retVal.ToList();
            }
            var uniToEnterFilter =
                m_UniversityWithcode.Where(w => w != universityId)
                    .Select(s => string.Format("({0} ne '{1}')", uniIdField, s));

            var val = string.Join(" and ", uniToEnterFilter);
            return val + string.Format(" or ({0}/any(t: t eq '{1}'))", userField, userId, uniIdField);
        }


    }

    public interface ISearchFilterProvider
    {
        Task<string> BuildFilterExpression(long universityId, string uniIdField, string userField, long userId);
    }
}
