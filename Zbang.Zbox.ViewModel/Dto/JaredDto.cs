using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class JaredDto
    {
        public IEnumerable<UniversityDto> Universities { get; set; }
        public Dictionary<string, IEnumerable<string>> Terms { get; set; }
    }

    public class UniversityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Short { get; set; }
    }
}
