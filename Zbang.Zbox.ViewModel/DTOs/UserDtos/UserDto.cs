﻿namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
    [System.Serializable]
    public class UserDto
    {
        public string Image { get; set; }
        public string LargeImage { get; set; }
        public string Name { get; set; }
        public long Uid { get; set; }

        public string Url { get; set; }
        
    }

    public class UserMinProfile
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
        public string UniversityName { get; set; }

        public string Url { get; set; }
    }

    [System.Serializable]
    public class UserDetailDto :UserDto
    {
        public UserDetailDto()
        {

        }
        public UserDetailDto(UserDetailDto t)
        {

            FirstTimeDashboard = t.FirstTimeDashboard;
            FirstTimeLibrary = t.FirstTimeLibrary;
            FirstTimeItem = t.FirstTimeItem;
            FirstTimeBox = t.FirstTimeBox;

            Email = t.Email;
            LibName = t.LibName;
            LibImage = t.LibImage;

            Image = t.Image;
            Name = t.Name;
            Uid = t.Uid;
        }
        public bool FirstTimeDashboard { get; set; }
        public bool FirstTimeLibrary { get; set; }
        public bool FirstTimeItem { get; set; }
        public bool FirstTimeBox { get; set; }

        // for Mobile
        public string Email { get; set; }
        public string LibName { get; set; }
        public string LibImage { get; set; }
    }

}
