using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "This is dto class format to json")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "This is dto class format to json")]
    public class JobDto : IShuffleable, IUrlRedirect
    {
        private string _responsibilities;
        private string _compensation;

        public string Title { get; set; }

        public string Responsibilities
        {
            get => _responsibilities.RemoveEndOfString(300);
            set => _responsibilities = value;
        }

        public DateTime DateTime { get; set; }

        public string Address { get; set; }

        public string CompensationType
        {
            get => _compensation.UppercaseFirst();
            set => _compensation = value;
        }

        public string Url { get; set; }

        public string Source => PrioritySource.ToString();

        public string Company { get; set; }


        [IgnoreDataMember]
        public PrioritySource PrioritySource { get; set; }

        [IgnoreDataMember]
        public int Order { get; set; }
    }
}
