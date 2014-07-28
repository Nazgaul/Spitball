using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Store
{
    public class AddBannersCommand : ICommand
    {
        public AddBannersCommand(IEnumerable<Banner> banner)
        {
            Banners = banner;
        }

        public IEnumerable<Banner> Banners { get; private set; }
    }

    public class Banner
    {
        public Banner(int id, string url, string imageUrl, int order)
        {
            Id = id;
            Url = url;
            ImageUrl = imageUrl;
            Order = order;
        }

        public int Id { get;private set; }
        public string Url { get; private set; }
        public string ImageUrl { get; private set; }
        public int Order { get; private set; }
    }
}
