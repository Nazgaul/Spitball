using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Zbang.Zbox.Infrastructure.Azure.Storage;

namespace Zbang.Zbox.Infrastructure.Azure.Table
{
    public class TableProvider : ITableProvider
    {
        internal const string UserRequests = "userrequests";
        internal const string FilterWords = "filterwords";

        private CloudTableClient m_TableClient;
        private CloudTableClient TableClient
        {
            get { return m_TableClient ?? (m_TableClient = StorageProvider.ZboxCloudStorage.CreateCloudTableClient()); }
        }

        private CloudTable UserRequestTable()
        {
            return TableClient.GetTableReference(UserRequests);
        }

        public async Task InsertUserRequestAsync(TableEntity entity)
        {
            try
            {
                TableOperation insertOperation = TableOperation.Insert(entity);

                await UserRequestTable().ExecuteAsync(insertOperation);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode == "EntityAlreadyExists")
                {
                    return;
                }
                throw;
            }
        }
    }
}
