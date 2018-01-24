using System.Xml.Serialization;

namespace Cloudents.Infrastructure.Write.Job.Entities
{
    [System.Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("job", Namespace = "", IsNullable = false)]
    public class CareerBuilderJobs
    {
        private string locationField;

        private string titleField;

       // private string cityField;

        private string stateField;

        //private ushort zipField;

        private string countryField;

        private string job_typeField;

        private System.DateTime posted_atField;

        private string job_referenceField;

        private string bodyField;

        private string companyField;

        private string mobile_friendly_applyField;

        private string categoryField;

        private string html_jobsField;

        private string urlField;

        private decimal cpaField;

        public string location
        {
            get
            {
                return locationField;
            }
            set
            {
                locationField = value;
            }
        }

        public string title
        {
            get
            {
                return titleField;
            }
            set
            {
                titleField = value;
            }
        }

         [XmlElement("city")]
        public string City
        {
            get;set;
        }

        public string state
        {
            get
            {
                return stateField;
            }
            set
            {
                stateField = value;
            }
        }

        [XmlElement("zip")]
        public string Zip
        {
            get; set;
        }

        public string country
        {
            get
            {
                return countryField;
            }
            set
            {
                countryField = value;
            }
        }

        public string job_type
        {
            get
            {
                return job_typeField;
            }
            set
            {
                job_typeField = value;
            }
        }

        [XmlElement(DataType = "date")]
        public System.DateTime posted_at
        {
            get
            {
                return posted_atField;
            }
            set
            {
                posted_atField = value;
            }
        }

        public string job_reference
        {
            get
            {
                return job_referenceField;
            }
            set
            {
                job_referenceField = value;
            }
        }

        public string body
        {
            get
            {
                return bodyField;
            }
            set
            {
                bodyField = value;
            }
        }

        public string company
        {
            get
            {
                return companyField;
            }
            set
            {
                companyField = value;
            }
        }

        public string mobile_friendly_apply
        {
            get
            {
                return mobile_friendly_applyField;
            }
            set
            {
                mobile_friendly_applyField = value;
            }
        }

        public string category
        {
            get
            {
                return categoryField;
            }
            set
            {
                categoryField = value;
            }
        }

        public string html_jobs
        {
            get
            {
                return html_jobsField;
            }
            set
            {
                html_jobsField = value;
            }
        }

        public string url
        {
            get
            {
                return urlField;
            }
            set
            {
                urlField = value;
            }
        }

        public decimal cpa
        {
            get
            {
                return cpaField;
            }
            set
            {
                cpaField = value;
            }
        }
    }
}
