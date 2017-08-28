
using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Dto.UserDtos;

namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{
    public class BoxDto2
    {
        private DateTime m_Date;
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

        public BoxPrivacySetting PrivacySetting { get; set; } // todo: this should be hidden

        public UserRelationshipType UserType { get; set; }

        public BoxType BoxType { get; set; }//new

        public Guid? DepartmentId { get; set; }
        //public string ShortUrl { get; set; }

    }

    public class BoxDtoWithMembers : BoxDto2
    {
        public IEnumerable<UserWithImageDto> People { get; set; }
    }
}
