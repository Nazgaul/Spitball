using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Data.Dapper
{
    public static class DapperConnection
    {

        internal const string ConnectionStringKey = "Zbox";

        public static async Task<IDbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default(CancellationToken), string connectionStringName = "Zbox")
        {
            var connection = new SqlConnection(ConfigFetcher.Fetch(connectionStringName));
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IDbConnection OpenConnection()
        {
            var connection = new SqlConnection(ConfigFetcher.Fetch(ConnectionStringKey));
            connection.Open();
            return connection;
        }
    }
}
