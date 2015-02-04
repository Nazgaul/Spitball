using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Zbang.Zbox.Infrastructure.Azure
{
    public interface ITableProvider
    {
        Task InsertUserRequestAsync(TableEntity entity);
    }
}
