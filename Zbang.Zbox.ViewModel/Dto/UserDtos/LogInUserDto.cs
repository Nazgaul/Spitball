﻿

using System;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class LogInUserDto : UserDetailDto
    {


        public long? UniversityData { get; set; }
    }


    public class ForgotPasswordDto
    {
        public string FirstName { get; set; }
        public string Culture { get; set; }
        public string GoogleId { get; set; }
        public long? FacebookId { get; set; }

        public Guid? IdentityId { get; set; }
    }
}
