using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Emails
{
    public class QuizDigestDto
    {
        public string UserName { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public long UserId { get; set; }
    }
}
