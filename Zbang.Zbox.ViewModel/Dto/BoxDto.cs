using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto
{
    [Serializable]
    public class BoxDto
    {

        //public BoxDto(long boxId, string boxName,
        //   UserRelationshipType userType, int itemCount, int membersCount,
        //    int commentCount, string courseCode, string professorName, 
        //    BoxType boxType, string uniName, string url)
        //{
        //    Id = boxId;
        //    Name = boxName;
        //    UserType = userType;
        //    ItemCount = itemCount;
        //    MembersCount = membersCount;
        //    CommentCount = commentCount;
        //    CourseCode = courseCode;
        //    Professor = professorName;
        //    BoxType = boxType;
        //    if (BoxType == BoxType.Academic)
        //    {
        //        UniName = uniName;
        //    }
        //    Url = url;
        //}
        public BoxDto(long id, string boxName, string boxPicture,
            UserRelationshipType userType, int itemCount, int membersCount, int commentCount,
           string courseCode, string professorName, BoxType boxType, string url)
        {
            Id = id;
            Name = boxName;
            UserType = userType;
            BoxType = boxType;
            ItemCount = itemCount;
            BoxPicture = boxPicture;
            MembersCount = membersCount;
            CommentCount = commentCount;
            CourseCode = courseCode;
            Professor = professorName;
            Url = url;
        }


        public string Name { get;  set; }
        public UserRelationshipType UserType { get; set; }
        public BoxType BoxType { get; set; }
        public int ItemCount { get;  set; }
        public int MembersCount { get;  set; }
        public int CommentCount { get;  set; }
        public string BoxPicture { get; set; }
        public long Id { get;  set; }
        public string CourseCode { get;  set; }
        public string Professor { get;  set; }
       // public string UniName { get; set; }

        public string Url { get; set; }


    }

}
