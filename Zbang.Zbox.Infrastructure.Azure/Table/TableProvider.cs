using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Zbang.Zbox.Infrastructure.Azure.Storage;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Storage.Entities;

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

        public IEnumerable<string> GetFileterWored()
        {
            var table = TableClient.GetTableReference(FilterWords);

            var rangeQuery = new TableQuery<StemmerWordRemoval>();

            var excecuting = table.ExecuteQuery(rangeQuery);
            return excecuting.Select(s => s.Word);
        }

    }
}
