using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Domain.DataAccess
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;
            ioc.RegisterType<IUserRepository, UserRepository>();
            ioc.RegisterType<IUniversityRepository, UniversityRepository>();
            ioc.RegisterType<IUserBoxRelRepository, UserBoxRelRepository>();
            ioc.RegisterType<IBoxRepository, BoxRepository>();
            ioc.RegisterType<IAcademicBoxRepository, AcademicBoxRepository>();

            ioc.RegisterType<IItemTabRepository, ItemTabRepository>();
            ioc.RegisterType<IQuestionRepository, QuestionRepository>();
            ioc.RegisterType<IItemRateRepository, ItemRateRepository>();
            ioc.RegisterType<IInviteRepository, InviteRepository>();
            ioc.RegisterType<IUpdatesRepository, UpdatesRepository>();
            ioc.RegisterType<IQuizRepository, QuizRepository>();

            ioc.RegisterType<IItemRepository, ItemRepository>();
        }
    }
}
