using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetStoreProductByCategoryQuery
    {
        public GetStoreProductByCategoryQuery(int? categoryId)
        {
            CategoryId = categoryId;
        }

        public int? CategoryId { get; private set; }
    }
}
