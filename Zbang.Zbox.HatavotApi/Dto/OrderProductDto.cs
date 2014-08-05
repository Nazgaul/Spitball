using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Store.Dto
{
    class OrderProductDto
    {
        public string Name { get; set; }
        public float Price { get; set; }

        public float Saleprice { get; set; }
        public string Image { get; set; }

        public string Catcode { get; set; }
        public string CatalogNumber { get; set; }

        public string SupplyTime { get; set; }

        public int ProdOrder { get; set; }


        public float ProfitPercent { get; set; }

        public int CurrencyType { get; set; }

        public int Vat { get; set; }

        public int ProducerId { get; set; }

        public float Coupon { get; set; }

        public float DeliveryPrice { get; set; }
    }
}
