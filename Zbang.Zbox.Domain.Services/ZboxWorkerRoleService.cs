using System;
using Dapper;
using NHibernate;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;


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
            InternalDbi();




        }

        private void InternalDbi()
        {

            using (UnitOfWork.Start())
            {
                var m_UserRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IUserRepository>();
                var m_InviteRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IRepository<InviteToBox>>();
                var m_IdGenerator = Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();
                var m_BoxRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IRepository<Box>>();
                var m_UserBoxRelRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IRepository<UserBoxRel>>();

               var retVal = UnitOfWork.CurrentSession.Connection.Query(
                    "select RecepientId,BoxId,SenderId from zbox.message where TypeOfMsg = 2");
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var obj in retVal)
                    {
                        try
                        {
                            long recipientId = obj.RecepientId, senderId = obj.SenderId, boxId = obj.BoxId;
                            var userType = m_UserRepository.GetUserToBoxRelationShipType(recipientId, boxId);
                            if (userType != UserRelationshipType.None)
                            {
                                continue;
                            }
                            Guid id = m_IdGenerator.GetId();
                            var recipientUser = m_UserRepository.Load(recipientId);
                            var box = m_BoxRepository.Load(boxId);
                            var sender = m_UserRepository.Load(senderId);

                            var newInvite = new UserBoxRel(recipientUser, box, UserRelationshipType.Invite);
                            var inviteToBoxExistingUser = new InviteToBox(id, sender, box, newInvite, null, null,
                                recipientUser.Email);

                            m_UserBoxRelRepository.Save(newInvite);
                            m_InviteRepository.Save(inviteToBoxExistingUser);
                        }
                        catch
                        {
                        }

                    }
                    tx.Commit();
                }

            }
            //        bool retVal = true;
            //        var dic = new Dictionary<University, Department>();
            //        while (retVal)
            //        {
            //            var users = UnitOfWork.CurrentSession.QueryOver<User>()
            //                .Where(w => w.University2 != null)
            //                .And(w => w.Department == null)
            //                .Take(2000)
            //                .List();
            //            retVal = false;
            //            using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
            //            {
            //                foreach (var user in users)
            //                {
            //                    Department department;
            //                    if (!dic.TryGetValue(user.University2, out department))
            //                    {

            //                        department =
            //                            UnitOfWork.CurrentSession.QueryOver<Department>()
            //                                .Where(w => w.University == user.University2)
            //                                .Take(1).SingleOrDefault();
            //                        dic.Add(user.University2, department);
            //                    }
            //                    if (department == null)
            //                    {
            //                        continue;
            //                    }
            //                    retVal = true;
            //                    user.Department = department;
            //                    UnitOfWork.CurrentSession.Save(user);

            //                }
            //                tx.Commit();
            //            }
            //        }

            //    }


            //    using (UnitOfWork.Start())
            //    {
            //        var universities = UnitOfWork.CurrentSession.QueryOver<University>().List();
            //        using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
            //        {
            //            foreach (var university in universities)
            //            {
            //                var count = UnitOfWork.CurrentSession.QueryOver<Department>()
            //                    .Where(w => w.University == university)
            //                    .Select(Projections.Sum<Department>(s => s.NoOfBoxes)).SingleOrDefault<int>();
            //                university.UpdateNumberOfBoxes(count);
            //                UnitOfWork.CurrentSession.Save(university);
            //            }
            //            tx.Commit();
            //        }
            //    }
        }

        public bool Dbi(int index)
        {
            bool retVal = false;

            using (UnitOfWork.Start())
            {
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    var universities = UnitOfWork.CurrentSession.QueryOver<University>().List();
                    var universityRepository = Infrastructure.Ioc.IocFactory.Unity.Resolve<IUniversityRepository>();
                    foreach (var university in universities)
                    {
                        university.UpdateNumberOfBoxes(universityRepository.GetNumberOfBoxes(university));
                        UnitOfWork.CurrentSession.Save(university);
                    }
                    tx.Commit();
                }
                //        TraceLog.WriteInfo("Processing departments");
                var departments = UnitOfWork.CurrentSession.QueryOver<Library>().List();

                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    foreach (var department in departments)
                    {

                        var x = UnitOfWork.CurrentSession.Get<Library>(department.Id);
                        x.UpdateNumberOfBoxes();
                        while (x != null)
                        {
                            UnitOfWork.CurrentSession.Save(x);
                            x = x.Parent;
                        }

                    }
                    tx.Commit();
                }
                UnitOfWork.CurrentSession.Connection.Execute("ReputationAdmin",
                    commandType: System.Data.CommandType.StoredProcedure);


              
            }
            return retVal;
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
                //UnitOfWork.CurrentSession.Delete("from StoreBanner e");
                //UnitOfWork.CurrentSession.Flush();
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


    }
}
