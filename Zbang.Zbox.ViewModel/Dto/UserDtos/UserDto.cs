﻿using System;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    [Serializable]
    public class UserDto
    {
        public string LargeImage { get; set; }
        public string Name {get;set;}
        public long Id { get; set; }

        public string Url { get; set; }

    }
    [Serializable]
    public class UserMinProfile
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
        public string UniversityName { get; set; }

        public string Url { get; set; }
    }

    public class UserWithImageDto
    {
        public long Id { get; set; }
        public string Image { get; set; }
    }

    public class UserWithImageNameDto
    {
        public long Id { get; set; }
        public string Image { get; set; }

        public string Name { get; set; }
    }
}
