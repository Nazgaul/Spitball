using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IUniversityReadSearchProvider
    {
        IEnumerable<string> SearchUniversity(string term);
    }

    public interface IUniversityWriteSearchProvider
    {
        void BuildUniversityData();
    }

}
