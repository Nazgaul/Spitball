using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Trace;


namespace Zbang.Zbox.Domain.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ZboxWorkerRoleService : IZboxWorkerRoleService
    {
        private readonly ICommandBus m_CommandBus;
        private readonly IWithCache m_Cache;

        public ZboxWorkerRoleService(ICommandBus commandBus, IWithCache cache)
        {
            m_CommandBus = commandBus;
            m_Cache = cache;
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
            
            // RemoveHtmlTags();

        }
        //delete top (5) from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0
        public Task<int> DeleteOldItemAsync(CancellationToken token)
        {
            return
                DeleteFromDbAsync(
                    "delete top (3) from zbox.item where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0", token);
            
        }
        public Task<int> DeleteOldBoxAsync(CancellationToken token)
        {
            return
                DeleteFromDbAsync(
                    "delete top (3) from zbox.box where isdeleted = 1 and updatetime < getutcdate() - 120 and isdirty = 0", token);

        }

        private async Task<int> DeleteFromDbAsync(string sql, CancellationToken token)
        {
            var needToLoop = true;
            var counter = 0;
            while (needToLoop)
            {
                using (var conn = await DapperConnection.OpenConnectionAsync(token))
                {
                    var i = await conn.ExecuteAsync(sql);
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                    needToLoop = i > 0;
                    counter += i;
                }
            }
            return counter;
        }

        public Task<int> DeleteOldUpdatesAsync(CancellationToken token)
        {
            return DeleteFromDbAsync("delete top (100)  from zbox.newUpdates where CreationTime < getutcdate() - 182 ", token);
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
        //public void RemoveHtmlTags()
        //{
        //    using (var unitOfWork = UnitOfWork.Start())
        //    {
        //        var query =
        //            UnitOfWork.CurrentSession.QueryOver<CommentReplies>()
        //                .Where(Restrictions.On<CommentReplies>(x => x.Text).IsLike("%<%")).Take(100);

        //        var comments = query.List();
        //        do
        //        {
        //            foreach (var comment in comments)
        //            {
        //                comment.Text = Infrastructure.TextManipulation.RemoveHtmlTags.Replace(comment.Text, string.Empty);

        //                UnitOfWork.CurrentSession.Save(comment);

        //            }
        //            unitOfWork.TransactionalFlush();
        //            comments = query.List();
        //        } while (comments.Count > 0);

        //    }
        //}

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
                    try
                    {
                        var command = new UpdateItemWithNoSizeCommand(itemIds);
                        await m_CommandBus.SendAsync(command);
                        unitOfWork.TransactionalFlush();
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError(ex);
                    }
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
                if (command.UniversityId > 0)
                {
                    await m_Cache.RemoveAsync(command);
                }
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



//        private void UpdateNumberOfBoxesInDepartmentNode()
//        {
//            var i = 0;
//            using (var unitOfWork = UnitOfWork.Start())
//            {
//                while (true)
//                {
//                    var libs = UnitOfWork.CurrentSession.Connection.Query<Guid>(
//                        @"select LibraryId from zbox.Library l 
//where l.LibraryId not in ( select l.ParentId from zbox.Library)
//and id = 166100
//order by LibraryId
//offset @pageNumber*50 ROWS
//    FETCH NEXT 50 ROWS ONLY", new { pageNumber = i });
//                    var libraryIds = libs as IList<Guid> ?? libs.ToList();
//                    if (libraryIds.Count == 0)
//                    {
//                        break;
//                    }
//                    foreach (var libraryId in libraryIds)
//                    {
//                        var library = UnitOfWork.CurrentSession.Load<Library>(libraryId);
//                        var libBoxes = library.UpdateNumberOfBoxes();
//                        var libNodes = library.UpdateNumberOfNodes();
//                        UnitOfWork.CurrentSession.Save(library);
//                        foreach (var libBox in libBoxes)
//                        {
//                            UnitOfWork.CurrentSession.Save(libBox);
//                        }
//                        foreach (var libBox in libNodes)
//                        {
//                            UnitOfWork.CurrentSession.Save(libBox);
//                        }
//                    }
//                    unitOfWork.TransactionalFlush();
//                    i++;
//                }
//            }
//        }





    }
}
