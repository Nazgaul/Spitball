using Cloudents.Core;
using NHibernate;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    public sealed class ReadonlyStatelessSession : IDisposable
    {
        public IStatelessSession Session { get; }

        private readonly ITransaction _transaction;

        public ReadonlyStatelessSession(IStatelessSession session)
        {
            Session = session;
            _transaction = Session.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        public void Dispose()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            Session?.Dispose();
        }
    }

    //https://stackoverflow.com/a/44853182/1235448
    //public interface IReadQuery
    //{
    //    Task<dynamic> QueryAsync(string sql, object param, CancellationToken token);
    //}

    //public sealed class DapperReadQuery : IReadQuery
    //{
    //    private readonly string _connectionString;

    //    public DapperReadQuery(DbConnectionStringProvider provider)
    //    {
    //        _connectionString = provider.GetConnectionString(Core.Enum.Database.System);

    //    }

    //    public async Task<dynamic> QueryAsync(string sql, object param, CancellationToken token)
    //    {
    //        using (var connection = new SqlConnection(_connectionString))
    //        {
    //            await connection.OpenAsync(token);
    //            using (var command = new SqlCommand(sql, connection))
    //            {
    //                foreach (var propertyInfo in param.GetType().GetProperties())
    //                {
    //                    command.Parameters.AddWithValue(propertyInfo.Name, propertyInfo.GetValue(param));
    //                }

    //                using (var reader = await command.ExecuteReaderAsync(token))
    //                {
    //                    while (reader.Read())
    //                    {


    //                    }
    //                }

    //            }


    //            return await connection.QueryAsync(sql, param);
    //        }
    //    }

    //    //public async Task<dynamic> QueryAsync2(string sql, object param, CancellationToken token)
    //    //{

    //    //    using (var connection = new SqlConnection(_connectionString))
    //    //    {
    //    //        await connection.OpenAsync(token);
    //    //        return await connection.QueryAsync(sql,);
    //    //    }
    //    //}
    //}
}