using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class StoreProductFeatures
    {
        protected StoreProductFeatures()
        {
        }

        public StoreProductFeatures(string category, string description, float price, StoreProduct product)
        {
            if (description == null) throw new ArgumentNullException("description");
            Category = category;
            Description = description.Trim();
            Price = price;
            Product = product;
        }

        public int Id { get; set; }
        public string Category { get; set; }

        public string Description { get; set; }

        public float Price { get; set; }

        public StoreProduct Product { get; set; }
    }
}
