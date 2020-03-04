using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Exceptions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ITransaction _transaction;

        public UnitOfWork(ISession session)
        {
            _transaction = session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void Dispose()
        {
            _transaction.Dispose();
            //unit of work should not dispose Session
            // _session.Dispose();
        }

        public async Task CommitAsync(CancellationToken token)
        {

            try
            {
                if (!_transaction.IsActive)
                {
                    throw new InvalidOperationException("No active transaction");
                }

                await _transaction.CommitAsync(token);
            }
            /*SELECT * FROM sys.messages
WHERE text like '%duplicate%' and text like '%key%' and language_id = 1033*/
            catch (GenericADOException ex) when (ex.InnerException is SqlException sql &&
                                                 (sql.Number == 2601 || sql.Number == 2627))
            {
                throw new DuplicateRowException("Duplicate row", sql);
            }
            catch (GenericADOException ex) when (ex.InnerException is SqlException sql &&
                                                 (sql.Number == 547))
            {
                throw new SqlConstraintViolationException("constraint Violation", sql);
            }
            catch (NonUniqueObjectException ex)
            {
                throw new DuplicateRowException("Duplicate row", ex);
            }


            //foreach (var @event in _eventStore)
            //{
            //    await _eventPublisher.PublishAsync(@event, token);
            //}
        }



        public async Task RollbackAsync(CancellationToken token)
        {
            if (_transaction.IsActive)
            {
                await _transaction.RollbackAsync(token);
            }
        }
    }
}
