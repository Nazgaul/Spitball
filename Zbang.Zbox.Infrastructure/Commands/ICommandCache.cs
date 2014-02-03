using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Commands
{
    public interface ICommandCache : ICommand
    {
        string[] GetCacheKeys();
    }
}
