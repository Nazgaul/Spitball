using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Zbang.Zbox.WcfRestService.Models
{
    [DataContract]
    public class ChangeFileName
    {
        [DataMember]
        public string NewFileName { get; set; }

        public override string ToString()
        {
            return string.Format("  NewFileName {0}",
                    NewFileName);

        }
    }
}