using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Data.Dapper
{
    public static class DapperConnection
    {
        static DapperConnection()
        {
            const string defaultRetryStrategyName = "fixed";
            const int retryCount = 10;
            var retryInterval = TimeSpan.FromSeconds(3);

            var strategy = new FixedInterval(defaultRetryStrategyName, retryCount, retryInterval);
            var strategies = new List<RetryStrategy> { strategy };
            var manager = new RetryManager(strategies, defaultRetryStrategyName);

            RetryManager.SetDefault(manager);
        }

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

        public static Task<IDbConnection> OpenReliableConnectionAsync(CancellationToken cancellationToken, string connectionStringName = ConnectionStringKey)
        {
            var retryPolicy = RetryManager.Instance.GetDefaultSqlConnectionRetryPolicy();
            //retryPolicy.Retrying += (sender, args) =>
            //{
            //    // Log details of the retry.
            //    var msg = $"Retry - Count:{args.CurrentRetryCount}, Delay:{args.Delay}, Exception:{args.LastException}";
            //    TraceLog.WriteWarning(msg);
            //};
            return retryPolicy.ExecuteAsync(() => OpenConnectionAsync(cancellationToken, connectionStringName), cancellationToken);
        }
    }
}
