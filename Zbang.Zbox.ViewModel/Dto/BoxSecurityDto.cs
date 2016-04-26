using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class BoxSecurityDto
    {
        public BoxPrivacySetting PrivacySetting { get; set; }

        public UserRelationshipType UserType { get; set; }

        public UserLibraryRelationType LibraryUserType { get; set; }

        public BoxType BoxType { get; set; }
    }
}
