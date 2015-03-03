using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{
    public class BoxToFriendDto
    {

        public UserRelationshipType UserType { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }

        public string Url { get; set; }

        public int ItemCount { get; set; }
        public int MembersCount { get; set; }
        public int CommentCount { get; set; }
        public string CourseCode { get; set; }
        public string Professor { get; set; }
        public BoxType BoxType { get; set; }
       
    }
}
