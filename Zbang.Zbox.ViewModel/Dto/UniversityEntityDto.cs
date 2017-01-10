using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class UniversityEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Extra { get; set; }
    }
}
