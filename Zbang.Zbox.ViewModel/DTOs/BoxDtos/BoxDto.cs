using System.Collections.Generic;
using User = Zbang.Zbox.ViewModel.DTOs.UserDtos;
using Zbang.Zbox.Infrastructure.Enums;
using System;
using Zbang.Zbox.ViewModel.DTOs.Library;

namespace Zbang.Zbox.ViewModel.DTOs.BoxDtos
{
    /// <summary>
    /// New box with detail dto - used for box  page in new design
    /// </summary>
    public class BoxDto
    {
        private string m_Image;
        public BoxDto()
        {
            Subscribers = new List<User.UserDto>();
            Tabs = new List<TabDto>();
        }
        public string Image
        {
            get { return m_Image; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                    m_Image = blobProvider.GetThumbnailUrl(value);
                }
            }
        }
        public string Name { get; private set; }
        public string OwnerName { get; private set; }

        public string UniCountry { get; set; }

        public long OwnerUid { get; private set; }
        public string CourseId { get; private set; }
        public string ProfessorName { get; set; }




        public long Items { get; set; }
        public long Comments { get; set; }

        public long Members { get; private set; }
        public IEnumerable<User.UserDto> Subscribers { get; set; }
        public IEnumerable<TabDto> Tabs { get; set; }

        public BoxPrivacySettings PrivacySetting { get; set; }
        public UserRelationshipType UserType { get; set; }
        public BoxType BoxType { get; set; }

        public NodeDto Parent;
    }
}
