using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface ITableProvider
    {
        Task InsertUserRequestAsync(TableEntity entity);
        IEnumerable<string> GetFileterWored();
    }
}
