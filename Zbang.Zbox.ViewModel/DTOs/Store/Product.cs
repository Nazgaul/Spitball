using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Store
{
   public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ExtraDetails { get; set; }
        public string NumberOfSales { get; set; }
        public string Coupon { get; set; }
        public string SalePrice { get; set; }
        public string PictureUrl { get; set; }
    }
}
