using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Infrastructure.Data
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;

            ioc.RegisterGeneric(typeof(IRepository<>), typeof(NHibernateRepository<>));
            ioc.RegisterGeneric(typeof(IDocumentDbRepository<>), typeof(DocumentDbRepository<>));
      

        }
    }
}
