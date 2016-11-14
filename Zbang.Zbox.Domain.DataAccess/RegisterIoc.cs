using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Domain.DataAccess
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;
            ioc.RegisterType<IUserRepository, UserRepository>();
            ioc.RegisterType<IUniversityRepository, UniversityRepository>();
            ioc.RegisterType<IUserBoxRelRepository, UserBoxRelRepository>();
            ioc.RegisterType<IUserLibraryRelRepository, UserLibraryRelRepository>();
            ioc.RegisterType<IBoxRepository, BoxRepository>();
            ioc.RegisterType<IAcademicBoxRepository, AcademicBoxRepository>();

            ioc.RegisterType<IItemTabRepository, ItemTabRepository>();
            ioc.RegisterType<IQuestionRepository, QuestionRepository>();
            ioc.RegisterType<IItemRateRepository, ItemRateRepository>();
            ioc.RegisterType<IInviteRepository, InviteRepository>();
            ioc.RegisterType<IUpdatesRepository, UpdatesRepository>();
            ioc.RegisterType<ICommentLikeRepository, CommentLikeRepository>();
            ioc.RegisterType<IReplyLikeRepository, ReplyLikeRepository>();
            //ioc.RegisterType<IQuizRepository, QuizRepository>();

            ioc.RegisterType<IItemRepository, ItemRepository>();
            ioc.RegisterType<IReputationRepository, ReputationRepository>();
            ioc.RegisterType<ILibraryRepository, LibraryRepository>();
            ioc.RegisterType<IChatUserRepository, ChatUserRepository>();
            ioc.RegisterType<IConnectionRepository, ConnectionRepository>();
            ioc.RegisterType<IFlashcardPinRepository, FlashcardPinRepository>();
        }
    }
}
