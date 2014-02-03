using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs.Library
{
    public class UniversityByPrefixDto
    {
        public string Name { get; private set; }
        public string Image { get; private set; }
        public long Uid { get; private set; }

        public bool NeedCode { get; private set; }
        public long MemberCount { get; private set; }
    }
}
