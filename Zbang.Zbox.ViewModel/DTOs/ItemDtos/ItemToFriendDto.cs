namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class ItemToFriendDto
    {
        public ItemToFriendDto(long id, string image, double rate, int numOfViews, string name, long boxId, string boxName,
            string url)
        {
            Id = id;
            Image = image;
            Rate = rate;
            NumOfViews = numOfViews;
            Name = name;
            BoxId = boxId;
            BoxName = boxName;
            Url = url;
        }
        public long Id { get; set; }
        public string Image { get; set; }
        public double Rate { get; set; }
        public int NumOfViews { get; set; }
        public string Name { get; set; }
        public long BoxId { get; set; }
        public string BoxName { get; set; }

        public string Url { get; set; }

    }
}
