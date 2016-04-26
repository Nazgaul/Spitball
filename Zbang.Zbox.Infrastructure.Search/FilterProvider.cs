﻿using System.Collections.Generic;
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
                var retVal = await m_UniversityWithCodeProvider.GetUniversityWithCodeAsync();
                m_UniversityWithcode = retVal.ToList();
            }
            var uniToEnterFilter =
                m_UniversityWithcode.Where(w => w != universityId)
                    .Select(s => $"({uniIdField} ne '{s}')");

            var val = "(" + string.Join(" and ", uniToEnterFilter) + " and (not(" + uniIdField + " eq '-1')))";
            return val + $" or ({userField}/any(t: t eq '{userId}')  ) ";
        }


    }

    public interface ISearchFilterProvider
    {
        Task<string> BuildFilterExpression(long universityId, string uniIdField, string userField, long userId);
    }
}
