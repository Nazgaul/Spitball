using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NHibernate.Linq;
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
            UpdateNumberOfBoxesInDepartmentNode();
            //UpdateMismatchUrl();
        }

        private void UpdateNumberOfBoxesInDepartmentNode()
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                var libs = UnitOfWork.CurrentSession.Query<Library>().Where(w => w.University.Id == 1177);
                foreach (var library in libs)
                {
                    library.UpdateNumberOfBoxes();
                    UnitOfWork.CurrentSession.Save(library);
                }
                unitOfWork.TransactionalFlush();
            }
        }

        /// <summary>
        /// this function is greated due to merge tool
        /// </summary>
        private void UpdateMismatchUrl()
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                const string sql1 = @"select top 10 itemid from zbox.item 
where CHARINDEX(CAST(boxid as varchar),url) <= 0
order by 1";
                const string sql2 = @"select id from zbox.quiz where CHARINDEX(CAST(boxid as varchar),url) <= 0
order by 1";

                var breakLoop = true;
                while (breakLoop)
                {
                    var quizzes = UnitOfWork.CurrentSession.Connection.Query<long>(sql2);

                    breakLoop = false;
                    //var items = UnitOfWork.CurrentSession.Query<Item>().Where(w => !w.IsDeleted && w.Url == null).ToList();
                    foreach (var dQuiz in quizzes)
                    {
                        breakLoop = true;
                        var quiz = UnitOfWork.CurrentSession.Load<Quiz>(dQuiz);
                        quiz.GenerateUrl();
                        quiz.IsDirty = true;
                        UnitOfWork.CurrentSession.Save(quiz);
                    }
                    
                    unitOfWork.TransactionalFlush();
                }
                breakLoop = true;
                while (breakLoop)
                {
                    var items = UnitOfWork.CurrentSession.Connection.Query<long>(sql1);

                    breakLoop = false;
                    //var items = UnitOfWork.CurrentSession.Query<Item>().Where(w => !w.IsDeleted && w.Url == null).ToList();
                    foreach (var dItem in items)
                    {
                        breakLoop = true;
                        var item = UnitOfWork.CurrentSession.Load<Item>(dItem);
                        item.GenerateUrl();
                        item.IsDirty = true;
                        UnitOfWork.CurrentSession.Save(item);
                    }

                    unitOfWork.TransactionalFlush();
                }

            }
        }

        public bool Dbi(int index)
        {
            DeleteOldUpdates();
            UpdateUniversityStats();
            UpdateMismatchUrl();
            return false;
        }

        private void DeleteOldUpdates()
        {
            using (UnitOfWork.Start())
            {
                var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteOldUpdates");
                query.ExecuteUpdate();
            }
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
