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

        //private string locationField;

        //private string titleField;

        //private string cityField;

        //private string stateField;

        //private ushort zipField;

        //private string countryField;

        //private string job_typeField;

        //private DateTime posted_atField;

        //private string job_referenceField;

        //private string companyField;

        //private string mobile_friendly_applyField;

        //private object categoryField;

        //private string html_jobsField;

        //private string urlField;

        //private string bodyField;

        //private decimal cpcField;

        //
        //public string location
        //{
        //    get
        //    {
        //        return locationField;
        //    }
        //    set
        //    {
        //        locationField = value;
        //    }
        //}

        
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("state")]
        public string State { get; set; }
      

        [XmlElement("zip")]
        public string Zip { get; set; }

        //public string country
        //{
        //    get
        //    {
        //        return countryField;
        //    }
        //    set
        //    {
        //        countryField = value;
        //    }
        //}

        
        [XmlElement("job_type")]
        public string JobType { get; set; }

        [XmlElement("posted_at")]
        public string PostedDate { get; set; }
        

        //public string job_reference
        //{
        //    get
        //    {
        //        return job_referenceField;
        //    }
        //    set
        //    {
        //        job_referenceField = value;
        //    }
        //}

        [XmlElement("company")]
        public string Company { get; set; }

        
        //public string mobile_friendly_apply
        //{
        //    get
        //    {
        //        return mobile_friendly_applyField;
        //    }
        //    set
        //    {
        //        mobile_friendly_applyField = value;
        //    }
        //}

        
        //public object category
        //{
        //    get
        //    {
        //        return categoryField;
        //    }
        //    set
        //    {
        //        categoryField = value;
        //    }
        //}

        
        //public string html_jobs
        //{
        //    get
        //    {
        //        return html_jobsField;
        //    }
        //    set
        //    {
        //        html_jobsField = value;
        //    }
        //}

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("body")]
        public string Body { get; set; }

        
        //public decimal cpc
        //{
        //    get
        //    {
        //        return cpcField;
        //    }
        //    set
        //    {
        //        cpcField = value;
        //    }
        //}
    }
   
}
