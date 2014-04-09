using NHibernate;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;


namespace Zbang.Zbox.Domain.Services
{
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

        public void Dbi()
        {
            using (UnitOfWork.Start())
            {

                var boxRepository = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<IBoxRepository>();
                //members count
                using (ITransaction tx = UnitOfWork.CurrentSession.BeginTransaction())
                {
                    //box members
                    var boxes = UnitOfWork.CurrentSession.QueryOver<Box>()
                                         .Where(w => w.IsDeleted == false)
                                         .List();
                    foreach (var box in boxes)
                    {
                        box.CalculateMembers();
                        box.UpdateItemCount();
                        box.UpdateQnACount(boxRepository.QnACount(box.Id));
                 //       box.CreateCreationQuestionIfNoneExists();
                        UnitOfWork.CurrentSession.Save(box);
                    }
                    tx.Commit();
                }
            }
        }

       
    }
}
