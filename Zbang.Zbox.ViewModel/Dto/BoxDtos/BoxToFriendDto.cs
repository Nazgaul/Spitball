using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{
    public class BoxToFriendDto
    {

        public UserRelationshipType UserType { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Picture {get;set;}

        public string Url { get; set; }
       
    }
}
