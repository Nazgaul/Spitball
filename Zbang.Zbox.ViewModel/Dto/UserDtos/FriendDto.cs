namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
   public class FriendDto
    {
       public FriendDto(string uid, string name, string image)
        {
            Name = name;
            Image = image;
            Uid = uid;
        }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Uid { get; set; }
    }
}
