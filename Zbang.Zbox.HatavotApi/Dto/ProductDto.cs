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

        public string Upgrade1 { get; set; }

        public string UpgradeValue1 { get; set; }
        public string Upgrade2 { get; set; }
        public string UpgradeValue2 { get; set; }

        public string Upgrade3 { get; set; }
        public string UpgradeValue3 { get; set; }

        public string Upgrade4 { get; set; }
        public string UpgradeValue4 { get; set; }

        public string Upgrade5 { get; set; }
        public string UpgradeValue5 { get; set; }

        public string Upgrade6 { get; set; }
        public string UpgradeValue6 { get; set; }


        public string NotActive { get; set; }

        public string UniversityId { get; set; }

        public int Order { get; set; }

        public int CategoryOrder { get; set; }
    }
}
