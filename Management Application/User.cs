using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Management_Application
{
    class User
    {
        public User()
        {
            UniversityName = "-None-";
        }
        public long UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string UserImageLarge { get; set; }
        public Sex Sex { get; set; }
        public UserType UserType { get; set; }
        public MobileOperatingSystem MobileDevice { get; set; }

        public DateTime CreationTime { get; set; }

        public string Url { get; set; }
        public long FacebookUserId { get; set; }
        public int Score { get; set; }
        public string Culture { get; set; }

        public string UniversityName { get; set; }

       
    }
}
