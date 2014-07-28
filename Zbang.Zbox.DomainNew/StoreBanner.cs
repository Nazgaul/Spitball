using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class StoreBanner
    {
        protected StoreBanner() { }

        public StoreBanner(int id, string url, string imageUrl, StoreBannerLocation location, int order)
        {
            Update(id, url, imageUrl, location, order);
        }

        public void Update(int id, string url, string imageUrl, StoreBannerLocation location, int order)
        {
            Id = id;
            Url = url;
            ImageUrl = imageUrl;
            Location = location;
            if (location == StoreBannerLocation.TopLeft)
            {
                Order = order;
            }
        }

        public int Id { get; private set; }

        public string Url { get; private set; }

        public string ImageUrl { get; private set; }

        public StoreBannerLocation Location { get; private set; }

        public int? Order { get; private set; }

    }
}
