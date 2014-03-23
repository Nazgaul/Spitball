using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs
{
    [Serializable]
    public class InviteDto
    {
        public Guid MsgId { get; set; }
        public long? BoxId { get; set; }
        public string UserPic { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public bool IsNew { get; set; }

        public string Message { get; set; }
       
        public string BoxName { get; private set; }
        public string Universityname { get; set; }

        public string Url { get; set; }
    }
}
