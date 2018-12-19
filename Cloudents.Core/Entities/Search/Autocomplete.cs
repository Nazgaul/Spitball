﻿using System.Runtime.Serialization;
using Cloudents.Application.Enum;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Entities.Search
{
    [DataContract]
    public class AutoComplete : ISearchObject
    {
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// The auto suggest list for the user
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Prefix { get; set; }

        /// <summary>
        /// The End result to search given a key
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public Vertical Vertical { get; set; }
    }
}