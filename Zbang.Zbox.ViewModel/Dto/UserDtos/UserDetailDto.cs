using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserDetailDto 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
       

        public string Url { get; set; }
        public string Culture { get; set; }

        public int Score { get; set; }

        public string UniversityCountry { get; set; }
        public string UniversityName { get; set; }
        public long? UniversityId { get; set; }

        public string Email { get; set; }
        public DateTime DateTime { get; set; }

        public bool IsAdmin { get; set; }

        public string Token { get; set; }


        public Sex Sex { get; set; }

       // public int Unread { get; set; }
    }
}