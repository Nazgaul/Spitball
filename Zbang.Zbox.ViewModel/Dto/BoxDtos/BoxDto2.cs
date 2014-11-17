
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{
    public class BoxDto2
    {
        private DateTime m_Date;
        public string Image { get; set; }
        public string Name { get; set; }
        public string CourseId { get; set; } 
        public string ProfessorName { get; set; } 
        public int Items { get; set; }

        public int Feeds { get; set; }

        public int Members { get; set; } 
        public int Quizes { get; set; }

        public string OwnerName { get; set; } 
        public long OwnerId { get; set; }

        public DateTime Date {
            get { return DateTime.SpecifyKind(m_Date, DateTimeKind.Utc); }
            set { m_Date = value; } }

  
        public BoxPrivacySettings PrivacySetting { get; set; } // todo: this should be hidden

        public UserRelationshipType UserType { get; set; }

        public BoxType BoxType { get; set; }//new

       
    }
}
