using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NHibernate.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
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
        }

        public bool Dbi(int index)
        {
            UpdateUniversityStats();
            return false;
        }

        private void UpdateUniversityStats()
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                var i = 0;
                var universitiesIds = UnitOfWork.CurrentSession.Query<University>()
                    .Where(s => s.IsDeleted == false).OrderBy(o => o.Id).Skip(i * 100).Take(100)
                    .Select(s => s.Id).ToList();
                do
                {
                    var command = new UpdateUniversityStatsCommand(universitiesIds);
                    m_CommandBus.Send(command);
                    unitOfWork.TransactionalFlush();
                    i++;
                    universitiesIds = UnitOfWork.CurrentSession.Query<University>()
                   .Where(s => s.IsDeleted == false).OrderBy(o => o.Id).Skip(i * 100).Take(100)
                   .Select(s => s.Id).ToList();
                } while (universitiesIds.Any());
            }
        }


        public void AddProducts(AddProductsToStoreCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void AddCategories(AddCategoriesCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
        }

        public void AddBanners(AddBannersCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                unitOfWork.TransactionalFlush();
            }
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
