using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs
{
    [Serializable]
    public class InviteDto
    {
       
        public long BoxUid { get; private set; }
        public string BoxName { get; private set; }
        public string BoxOwner { get; private set; }
        public string Universityname { get; set; }

        public string Url { get; set; }
    }
}
