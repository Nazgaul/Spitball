using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;
            ioc.RegisterType<IUserRepository, UserRepository>();
            ioc.RegisterType<IUserBoxRelRepository, UserBoxRelRepository>();
            ioc.RegisterType<IBoxRepository, BoxRepository>();
            ioc.RegisterType<IUniversityRepository, UniversityRepository>();
            ioc.RegisterType<IAcademicBoxRepository, AcademicBoxRepository>();
            //ioc.RegisterType<IActionRepository, ActionRepository>();

            //ioc.RegisterType<IBoxTabRepository, BoxTabRepository>();
            ioc.RegisterType<IItemTabRepository, ItemTabRepository>();
            ioc.RegisterType<IQuestionRepository, QuestionRepository>();
            //ioc.RegisterType<IAnswerRatingRepository, AnswerRatingRepository>();
            ioc.RegisterType<IItemRateRepository, ItemRateRepository>();
            ioc.RegisterType<IInviteRepository, InviteRepository>();
            ioc.RegisterType<IInviteToCloudentsRepository, InviteToCloudentsRepository>();
            ioc.RegisterType<IUpdatesRepository, UpdatesRepository>();
            ioc.RegisterType<IMessageBaseRepository, MessageBaseRepository>();

        }
    }
}
