using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Zbang.Zbox.WcfRestService.Models
{
    [DataContract]
    public class CreateFile
    {
        [DataMember]
        public string BlobName { get; set; }

        public override string ToString()
        {
            return string.Format("  BlobName {0}",
                    BlobName);
        }
    }
}