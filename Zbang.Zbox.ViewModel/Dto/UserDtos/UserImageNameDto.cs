namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserImageNameDto
    {
        public long Id { get; set; }
        public string Image { get; set; }

        public string Name { get; set; }
    }

    public class UserOnlineDto
    {
        public long Id { get; set; }
        public string Image { get; set; }

        public string Name { get; set; }

        public bool Online { get; set; }
    }
}