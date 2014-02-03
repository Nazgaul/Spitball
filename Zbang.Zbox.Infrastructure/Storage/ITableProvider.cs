using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage.Entities;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface ITableProvider
    {
        Task InsertUserRequestAsync(TableEntity entity);
        IEnumerable<string> GetFileterWored();
    }
}
