using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.DTOs.Library;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IUniversityReadSearchProvider
    {
        IEnumerable<UniversityByPrefixDto> SearchUniversity(string term);
    }

    public interface IUniversityWriteSearchProvider
    {
        void BuildUniversityData();
    }

}
