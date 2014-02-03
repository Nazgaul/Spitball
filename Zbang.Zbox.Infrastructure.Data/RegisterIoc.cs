using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.Data
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var Ioc = IocFactory.Unity;

            Ioc.RegisterType(typeof(Zbox.Infrastructure.Repositories.IRepository<>),typeof(Repositories.NHibernateRepository<>));
      

        }
    }
}
