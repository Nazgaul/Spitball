using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Zbang.Zbox.WorkerRoleSearch
{


    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("job", Namespace = "", IsNullable = false)]
   public  class WayUpJob
    {

        public string Id { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("ompany")]
        public string Company { get; set; }

        [XmlElement("posted_date")]
        public string PostedDate { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        /// <remarks/>
        [XmlElement("clickcastid")]

        public jobClickcastid[] ClickCastId { get; set; }

        /// <remarks/>
        [XmlElement("responsibilities")]
        public string Responsibilities { get; set; }

        /// <remarks/>
        [XmlElement("company_id")]
        public uint CompanyId { get; set; }

        /// <remarks/>
        [XmlElement("job_id")]
        public string JobId { get; set; }

        /// <remarks/>
        [XmlElement("city")]
        public string City { get; set; }

        /// <remarks/>
        [XmlElement("state")]
        public string State { get; set; }

        /// <remarks/>
        [XmlElement("zip")]
        public string Zip { get; set; }

        /// <remarks/>
        [XmlElement("country")]
        public string Country { get; set; }

        /// <remarks/>
        [XmlElement("gradyear")]
        public string GradYear { get; set; }

        /// <remarks/>
        [XmlElement("jobtype")]
        public string JobType { get; set; }

        /// <remarks/>
        [XmlElement("applicationtype")]
        public string ApplicationType { get; set; }

        /// <remarks/>
        [XmlElement("comptype")]
        public string CompType { get; set; }

        /// <remarks/>
        [XmlElement("biztype")]
        public string BizType { get; set; }

        /// <remarks/>
        [XmlElement("locationtype")]
        public string LocationType { get; set; }

        /// <remarks/>
        [XmlElement("bidtype")]
        public string BidType { get; set; }
    }

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class jobClickcastid
    {

        /// <remarks/>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlText]
        public string Value { get; set; }
    }
}
