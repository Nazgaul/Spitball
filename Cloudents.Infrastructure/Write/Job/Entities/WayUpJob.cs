using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Cloudents.Infrastructure.Write.Job.Entities
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

        [XmlElement("clickcastid")]

        public jobClickcastid[] ClickCastId { get; set; }

        [XmlElement("responsibilities")]
        public string Responsibilities { get; set; }

        [XmlElement("company_id")]
        public uint CompanyId { get; set; }

        [XmlElement("job_id")]
        public string JobId { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("state")]
        public string State { get; set; }

        [XmlElement("zip")]
        public string Zip { get; set; }

        [XmlElement("country")]
        public string Country { get; set; }

        [XmlElement("gradyear")]
        public string GradYear { get; set; }

        [XmlElement("jobtype")]
        public string JobType { get; set; }

        [XmlElement("applicationtype")]
        public string ApplicationType { get; set; }

        [XmlElement("comptype")]
        public string CompType { get; set; }

        [XmlElement("biztype")]
        public string BizType { get; set; }

        [XmlElement("locationtype")]
        public string LocationType { get; set; }

        [XmlElement("bidtype")]
        public string BidType { get; set; }
    }

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class jobClickcastid
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
