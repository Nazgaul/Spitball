using System;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserDetailDto 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
       
        public bool FirstTimeDashboard { get; set; }
        public bool FirstTimeLibrary { get; set; }
        public bool FirstTimeItem { get; set; }
        public bool FirstTimeBox { get; set; }

        public int Score { get; set; }

        public string Url { get; set; }

        public Guid? DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        //// for Mobile
        public string Email { get; set; }
        public string LibName { get; set; }
        public string LibImage { get; set; }
    }
}