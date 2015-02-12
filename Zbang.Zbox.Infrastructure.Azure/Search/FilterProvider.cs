using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class FilterProvider
    {
        private readonly IUniversityWithCode m_UniversityWithCodeProvider;
        private List<long> m_UniversityWithcode;
        protected FilterProvider(IUniversityWithCode universityWithCode)
        {
            m_UniversityWithCodeProvider = universityWithCode;
        }

        public async Task<string> BuildFilterExpression(long universityId, string uniIdField)
        {
            if (m_UniversityWithcode == null)
            {
                var retVal = await m_UniversityWithCodeProvider.GetUniversityWithCode();
                m_UniversityWithcode = retVal.ToList();
            }
            return string.Empty;

        }

        
    }
}
