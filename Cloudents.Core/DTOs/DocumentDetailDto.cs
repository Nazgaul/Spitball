﻿using System;
using System.IO;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class DocumentDetailDto
    {
        private string _name;

        [DataMember]
        public long Id{ get; set; }

        [DataMember]
        public string Name
        {
            get => Path.GetFileNameWithoutExtension(_name);
            set => _name = value;
        }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string University { get; set; }

        [DataMember]
        public string Course { get; set; }

        [DataMember]
        public string Professor { get; set; }

       


        [DataMember]
        public UserDto User { get; set; }

        [DataMember]
        public string Extension => Path.GetExtension(_name)?.TrimStart('.');

        //[DataMember]
        public DocumentType? TypeStr { get; set; }

        [DataMember]
        public string Type => TypeStr?.ToString("G");

        [DataMember]
        public int Pages { get; set; }

        [DataMember]
        public int Views { get; set; }

        [DataMember]
        public int Downloads { get; set; }

        [DataMember]
        public decimal? Price { get; set; }

        [DataMember]
        public bool IsPurchased { get; set; }

        //[DataMember]
        //public string Blob { get; set; }

        //public string Type { get; set; }
    }
}
