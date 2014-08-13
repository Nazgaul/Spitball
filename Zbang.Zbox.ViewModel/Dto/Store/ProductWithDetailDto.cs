
namespace Zbang.Zbox.ViewModel.Dto.Store
{
    public class ProductWithDetailDto
    {
        public string Name { get; set; }
        public string ExtraDetails { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public float DeliveryPrice { get; set; }
        public string CatalogNumber { get; set; }
        public int NumberofPayments { get; set; }
        public string SupplyTime { get; set; }

        public float TotalPrice { get; set; }

        public string Picture { get; set; }

        public long Id { get; set; }

        public string ProducerName { get; set; }
    }
}
