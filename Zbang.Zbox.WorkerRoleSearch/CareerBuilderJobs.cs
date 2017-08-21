using System.Xml.Serialization;

namespace Zbang.Zbox.WorkerRoleSearch
{
    /// <remarks/>
    [System.Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot("job", Namespace = "", IsNullable = false)]
    public class CareerBuilderJobs
    {
        private string locationField;

        private string titleField;

        private string cityField;

        private string stateField;

        private ushort zipField;

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

        /// <remarks/>
        public string location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
         [XmlElement("city")]
        public string City
        {
            get;set;
        }

        /// <remarks/>
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        [XmlElement("zip")]
        public string Zip
        {
            get; set;
        }

        /// <remarks/>
        public string country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string job_type
        {
            get
            {
                return this.job_typeField;
            }
            set
            {
                this.job_typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime posted_at
        {
            get
            {
                return this.posted_atField;
            }
            set
            {
                this.posted_atField = value;
            }
        }

        /// <remarks/>
        public string job_reference
        {
            get
            {
                return this.job_referenceField;
            }
            set
            {
                this.job_referenceField = value;
            }
        }

        /// <remarks/>
        public string body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }

        /// <remarks/>
        public string company
        {
            get
            {
                return this.companyField;
            }
            set
            {
                this.companyField = value;
            }
        }

        /// <remarks/>
        public string mobile_friendly_apply
        {
            get
            {
                return this.mobile_friendly_applyField;
            }
            set
            {
                this.mobile_friendly_applyField = value;
            }
        }

        /// <remarks/>
        public string category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }

        /// <remarks/>
        public string html_jobs
        {
            get
            {
                return this.html_jobsField;
            }
            set
            {
                this.html_jobsField = value;
            }
        }

        /// <remarks/>
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        public decimal cpa
        {
            get
            {
                return this.cpaField;
            }
            set
            {
                this.cpaField = value;
            }
        }
    }
}
