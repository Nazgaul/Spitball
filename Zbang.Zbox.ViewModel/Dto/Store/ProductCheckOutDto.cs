﻿
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Store
{
    public class ProductCheckOutDto
    {
        public string Name { get; set; }
        public string ExtraDetails { get; set; }
        public float DeliveryPrice { get; set; }

        public string Picture { get; set; }

        public float SalePrice { get; set; }
        public float Coupon { get; set; }

        public int NumberOfPayments { get; set; }

        public IEnumerable<ProductFeatures> Features { get; set; }
    }
}
