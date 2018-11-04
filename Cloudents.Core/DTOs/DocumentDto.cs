using System;
using System.IO;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class DocumentDto
    {
        [DataMember]

        public string Name { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string University { get; set; }

        [DataMember]
        public string Course { get; set; }

        [DataMember]
        public string Professor { get; set; }


        [DataMember]
        public string Owner { get; set; }

        [DataMember]
        public string Extension => Path.GetExtension(Blob)?.TrimStart('.');

        [DataMember]
        public DocumentType Type { get; set; }

        [DataMember]
        public int Pages { get; set; }

        [DataMember]
        public int Views { get; set; }


        //[DataMember]
        public string Blob { get; set; }

        //public string Type { get; set; }
    }
}
