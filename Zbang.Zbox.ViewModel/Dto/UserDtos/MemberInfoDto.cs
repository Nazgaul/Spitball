
namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
    public class MemberInfoDto
    {
        public MemberInfoDto(string uid, string name, string image, long boxesCount, long itemCount, long memberCount)
        {
            Uid = uid;
            Name = name;
            Image = image;
            BoxesCount = boxesCount;
            ItemCount = itemCount;
            MemberCount = memberCount;
        }
        public string Uid { get; private set; }
        public string Name { get; private set; }
        public string Image { get; private set; }
        public long BoxesCount { get; private set; }
        public long ItemCount { get; private set; }
        public long MemberCount { get; private set; }
    }
}
