using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class QuizSearchDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string BoxName { get; set; }

        public IEnumerable<string> Questions { get; set; }
        public IEnumerable<string> Answers { get; set; }
        public IEnumerable<long> UserIds { get; set; }
        public long BoxId { get; set; }
        public long? UniversityId { get; set; }
        public string Url { get; set; }
        public string UniversityName { get; set; }

    }
}
