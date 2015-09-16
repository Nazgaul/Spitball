using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserProfileImageCommand : ICommand
    {
        public UpdateUserProfileImageCommand(long userId, string imageUrl)
        {
            ImageUrl = imageUrl;
            UserId = userId;
        }

        public long UserId { get; private set; }
        public string ImageUrl { get; private set; }
    }
}
