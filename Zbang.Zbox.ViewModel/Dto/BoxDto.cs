using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto
{
    [Serializable]
    public class BoxDto
    {
        public BoxDto()
        {

        }
        public BoxDto(long id, string boxName,
            UserRelationshipType userType, int itemCount, int membersCount, int commentCount,
            string courseCode, string professorName, BoxType boxType, string url, Guid departmentId)
        {
            Id = id;
            Name = boxName;
            UserType = userType;
            BoxType = boxType;
            ItemCount = itemCount;
            MembersCount = membersCount;
            CommentCount = commentCount;
            CourseCode = courseCode;
            Professor = professorName;
            Url = url;
            DepartmentId = departmentId;
        }

        public string Name { get; set; }
        public UserRelationshipType UserType { get; set; }
        public BoxType BoxType { get; set; }
        public int ItemCount { get; set; }

        public Guid DepartmentId { get; set; }

        public int MembersCount { get; set; }
        public int CommentCount { get; set; }
        public long Id { get; set; }
        public string CourseCode { get; set; }
        public string Professor { get; set; }
        public string Url { get; set; }
    }
}
