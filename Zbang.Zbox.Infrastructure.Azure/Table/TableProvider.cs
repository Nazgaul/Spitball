using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage.Entities;

namespace Zbang.Zbox.Infrastructure.Storage
{
    class TableProvider : ITableProvider
    {
        internal const string UserRequests = "userrequests";
        internal const string FilterWords = "filterwords";

        private CloudTableClient m_TableClient;
        private CloudTableClient TableClient
        {
            get
            {
                if (m_TableClient == null)
                {
                    m_TableClient = StorageProvider.ZboxCloudStorage.CreateCloudTableClient();
                }
                return m_TableClient;
            }
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

            TableQuery<StemmerWordRemoval> rangeQuery = new TableQuery<StemmerWordRemoval>();

            var excecuting = table.ExecuteQuery<StemmerWordRemoval>(rangeQuery);
            return excecuting.Select(s => s.Word);
        }

    }
}
