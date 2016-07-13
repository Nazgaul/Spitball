using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;


namespace Zbang.Zbox.Domain.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ZboxWorkerRoleService : IZboxWorkerRoleService
    {
        private readonly ICommandBus m_CommandBus;

        public ZboxWorkerRoleService(ICommandBus commandBus)
        {
            m_CommandBus = commandBus;
        }

        public void UpdateThumbnailPicture(UpdateThumbnailCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void OneTimeDbi()
        {
            using (UnitOfWork.Start())
            {
                var items = UnitOfWork.CurrentSession.QueryOver<Item>().Where(w => !w.IsDeleted && w.Url == null).List();
                foreach (var item in items)
                {
                    item.GenerateUrl();
                    UnitOfWork.CurrentSession.Save(item);
                }
                UnitOfWork.Current.TransactionalFlush();
            }
        }


        public int DeleteOldUpdates()
        {

            var needToLoop = true;
            var counter = 0;
            while (needToLoop)
            {
                using (UnitOfWork.Start())
                {
                    var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteOldUpdates");
                    var result = query.ExecuteUpdate();
                    needToLoop = result > 0;
                    counter += result;
                }
            }
            using (UnitOfWork.Start())
            {
                var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteNotConnectedUpdates");
                var result = query.ExecuteUpdate();
                counter += result;
            }
            return counter;
        }


        public void UpdateUniversityStats(DateTime dateTime)
        {

            using (var unitOfWork = UnitOfWork.Start())
            {
                var i = 0;
                var query = UnitOfWork.CurrentSession.GetNamedQuery("UniversityData");
                query.SetDateTime("dateTime", dateTime);
                query.SetMaxResults(100);
                query.SetFirstResult(i * 100);
                var universitiesIds = query.List<long>();
                do
                {
                    var command = new UpdateUniversityStatsCommand(universitiesIds);
                    m_CommandBus.Send(command);
                    unitOfWork.TransactionalFlush();
                    i++;
                    query = UnitOfWork.CurrentSession.GetNamedQuery("UniversityData");
                    query.SetDateTime("dateTime", dateTime);
                    query.SetMaxResults(100);
                    query.SetFirstResult(i * 100);
                    universitiesIds = query.List<long>();
                } while (universitiesIds.Any());
            }
        }

        public async Task<long> UpdateFileSizesAsync(Action callback)
        {
            var count = 0;
            using (var unitOfWork = UnitOfWork.Start())
            {
                var query = UnitOfWork.CurrentSession.GetNamedQuery("ItemWithNoSize");
                query.SetMaxResults(100);
                var itemIds = query.List<long>();
                
                do
                {
                    count += itemIds.Count;
                    var command = new UpdateItemWithNoSizeCommand(itemIds);
                    await m_CommandBus.SendAsync(command);
                    unitOfWork.TransactionalFlush();
                    callback();
                    query = UnitOfWork.CurrentSession.GetNamedQuery("ItemWithNoSize");
                    query.SetMaxResults(100);
                    itemIds = query.List<long>();
                } while (itemIds.Any());

            }
            return count;
        }




        public void UpdateReputation(UpdateReputationCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public async Task UpdateSearchUniversityDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "update zbox.University set isdirty = 0 where id in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text);
            }
        }

        public async Task UpdateSearchBoxDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "update zbox.Box set isdirty = 0 where boxid in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text);
            }
        }
        public async Task UpdateSearchItemDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "update zbox.Item set isdirty = 0 where itemid in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text);
            }
        }

        public async Task UpdateSearchQuizDirtyToRegularAsync(UpdateDirtyToRegularCommand command)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql = "update zbox.Quiz set isdirty = 0 where id in @Ids";
                await conn.ExecuteAsync(sql, new { command.Ids }, commandType: CommandType.Text);
            }
        }

        public void UpdateUserFromUnsubscribe(UnsubscribeUsersFromEmailCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }


        public async Task DeleteBoxAsync(DeleteBoxCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                unitOfWork.TransactionalFlush();
            }
        }


        public async Task AddNewUpdateAsync(AddNewUpdatesCommand command)
        {

            using (var unitOfWork = UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                unitOfWork.TransactionalFlush();
            }
        }




        public void Statistics(UpdateStatisticsCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void UpdateQuota(UpdateQuotaCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }


    }
}
