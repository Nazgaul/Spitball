using Zbang.Zbox.Domain.Commands;
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

       
    }
}
