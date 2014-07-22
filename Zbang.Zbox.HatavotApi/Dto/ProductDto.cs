namespace Zbang.Zbox.Store.Dto
{
    public class ProductDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float Saleprice { get; set; }
        public string Image { get; set; }
        public string ExtraDetails { get; set; }
        public float Coupon { get; set; }

        public string CategoryCode { get; set; }

        public string Featured { get; set; } 

        public string Description { get; set; } 
        public string SupplyTime { get; set; } 

        public int ProductPayment { get; set; } 

        public string CatalogNumber { get; set; } 
        public float DeliveryPrice { get; set; }

        public string ProducerName { get; set; }
    }
}
