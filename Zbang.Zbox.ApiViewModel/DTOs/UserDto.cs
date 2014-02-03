using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ApiViewModel.DTOs
{
    [DataContract]
    public class UserDto
    {
        private UserType m_UserType;
        public UserDto(string name, long uid, string image, UserType userType)
        {
            Name = name;
            Uid = uid;
            Image = image;
            m_UserType = userType;
        }
        public UserDto()
        {

        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public long Uid { get; set; }
        [DataMember]
        public string Image { get; set; }
        [DataMember]
        public string UserType
        {
            get
            {
                return m_UserType.ToString();
            }
            private set { }
        }
    }
}
