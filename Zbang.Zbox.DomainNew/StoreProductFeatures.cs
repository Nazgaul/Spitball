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

        public StoreProductFeatures(string category, string description, float? price, StoreProduct product)
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

        public float? Price { get; set; }

        public StoreProduct Product { get; set; }


        public override int GetHashCode()
        {
            unchecked
            {
                var result = 11 * Category.GetHashCode();
                result += 13 * Description.GetHashCode();
                return result;
            }
        }
        public override bool Equals(object other)
        {
            if (this == other) return true;

            var feature = other as StoreProductFeatures;
            if (feature == null) return false;

            return String.Equals(Category.Trim(), feature.Category.Trim(), StringComparison.InvariantCultureIgnoreCase)
                   &&
                   String.Equals(Description.Trim(), feature.Description.Trim(),
                       StringComparison.InvariantCultureIgnoreCase);

        }
    }
}
