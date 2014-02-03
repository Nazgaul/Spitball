using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Url
{
    //for future use
    public interface IBoxUrlBuilder
    {
        string BoxName { get; }
        long Id { get; set; }
        string UniversityName { get; }
    }
}
