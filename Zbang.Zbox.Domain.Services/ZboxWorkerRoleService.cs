﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Proxy;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Trace;


namespace Zbang.Zbox.Domain.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class ZboxWriteService
    {


        public void UpdateThumbnailPicture(UpdateThumbnailCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void OneTimeDbi()
        {
            using (UnitOfWork.Start())
            {
                UpdateAllUrlsInSystem(); // this can work
            }
        }


        private void UpdateAllUrlsInSystem()
        {
            int i = 667;
            TraceLog.WriteInfo("starting on item");
            var items = UnitOfWork.CurrentSession.QueryOver<Item>()
                //.Where(Restrictions.Gt(Projections.SqlFunction("CHARINDEX",
                //    NHibernateUtil.String,
                //    Projections.Constant("("),
                //    Projections.Property<Item>(p => p.Url)
                //    ),
                //    0))
                .And(w => w.IsDeleted == false)
                .OrderBy(o => o.Id).Asc
                .Skip(i * 100).Take(100).List();
            do
            {
                TraceLog.WriteInfo("item index " + i);
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in items)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                items = UnitOfWork.CurrentSession.QueryOver<Item>()
                    .Where(w => w.IsDeleted == false)
                    .OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (items.Count > 0);

            i = 0;
            TraceLog.WriteInfo("starting on box");
            var boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
                .Where(w => w.IsDeleted == false)
                .OrderBy(o => o.Id).Asc
                .Skip(i * 100).Take(100).List();
            do
            {
                TraceLog.WriteInfo("box index " + i);
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in boxes)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
                    .Where(w => w.IsDeleted == false)
                    .OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (boxes.Count > 0);

           i = 0;
            TraceLog.WriteInfo("starting on quiz");
            var quizzes = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                .Where(w=>w.Publish)
                .OrderBy(o => o.Id).Asc
                .Skip(i * 100).Take(100).List();
            do
            {
                TraceLog.WriteInfo("quiz index " + i);
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in quizzes)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                quizzes = UnitOfWork.CurrentSession.QueryOver<Quiz>()
                    .Where(w => w.Publish)
                    .OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (quizzes.Count > 0);

            TraceLog.WriteInfo("starting on library");
            i = 0;
            var library = UnitOfWork.CurrentSession.QueryOver<Library>().OrderBy(o => o.Id).Asc
                .Skip(i * 100).Take(100).List();
            do
            {
                TraceLog.WriteInfo("library index " + i);
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in library)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                library = UnitOfWork.CurrentSession.QueryOver<Library>().OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (library.Count > 0);

            TraceLog.WriteInfo("starting on user");
            var users = UnitOfWork.CurrentSession.QueryOver<User>().OrderBy(o => o.Id).Asc
               .Skip(i * 100).Take(100).List();
            do
            {
                TraceLog.WriteInfo("user index " + i);
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var item in users)
                    {
                        item.GenerateUrl();
                        UnitOfWork.CurrentSession.Save(item);
                    }
                    tx.Commit();
                    i++;
                }
                users = UnitOfWork.CurrentSession.QueryOver<User>().OrderBy(o => o.Id).Asc.Skip(i * 100).Take(100).List();
            } while (users.Count > 0);
        }

        public bool Dbi(int index)
        {
            UpdateAdminReputation();
            return false;
        }

        private void UpdateAdminReputation()
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                var universitiesIds = conn.Query("SELECT id FROM zbox.university");
                foreach (var universitiesId in universitiesIds)
                {
                    const string sql = @"with topreputation as (
                        select userreputation , ROW_NUMBER() OVER(ORDER BY userreputation DESC) AS Row
                         from zbox.users u
                        where universityid = @id
                        )
                        update zbox.University set AdminScore = (
                        select  top 1 UserReputation  from topreputation 
                        where Row <= (select AdminNoOfPeople from zbox.University where id = @id)
                        order by Row desc)
                        where id = @id;";

                    conn.Execute(sql, new { universitiesId.id });
                }
            }
        }


        public void AddProducts(AddProductsToStoreCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void AddCategories(AddCategoriesCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void AddBanners(AddBannersCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void UpdateReputation(UpdateReputationCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
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


    }
}
