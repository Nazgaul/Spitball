namespace Zbang.Zbox.ViewModel.Dto.Store
{
   public class ProductDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ExtraDetails { get; set; }
        public string NumberOfSales { get; set; }
        public float Coupon { get; set; }
        public float SalePrice { get; set; }
        public string PictureUrl { get; set; }

       public string Url { get; set; }
    }
}
