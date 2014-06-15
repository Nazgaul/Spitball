using Microsoft.WindowsAzure.Storage.Table;
using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Storage.Entities
{
    public class NewUniversity : TableEntity
    {
        private SchoolType m_SchoolType ;
        public NewUniversity(string universityName, long userId, string country, SchoolType schoolType)
            :base("NewUniversity",universityName)
        {
            
            UserId = userId;
            Country = country;
            m_SchoolType = schoolType;
        }
        public NewUniversity()
        {
        }

        public long UserId { get;  set; }
        public string Country { get;  set; }


        public string SchoolType
        {
            get
            {
                return m_SchoolType.ToString("g");
            }
            set
            {
                Enum.TryParse(value, out m_SchoolType);
            }
        }


    }
}
