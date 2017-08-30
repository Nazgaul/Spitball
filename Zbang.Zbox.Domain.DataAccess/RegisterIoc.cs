using Autofac;

namespace Zbang.Zbox.Domain.DataAccess
{
    
    public class RepositoryModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<UniversityRepository>().As<IUniversityRepository>();
            builder.RegisterType<UserBoxRelRepository>().As<IUserBoxRelRepository>();
            builder.RegisterType<UserLibraryRelRepository>().As<IUserLibraryRelRepository>();
            builder.RegisterType<BoxRepository>().As<IBoxRepository>();
            builder.RegisterType<AcademicBoxRepository>().As<IAcademicBoxRepository>();
            builder.RegisterType<ItemTabRepository>().As<IItemTabRepository>();
            builder.RegisterType<QuestionRepository>().As<IQuestionRepository>();
            builder.RegisterType<ItemRateRepository>().As<IItemRateRepository>();
            builder.RegisterType<InviteRepository>().As<IInviteRepository>();
            builder.RegisterType<UpdatesRepository>().As<IUpdatesRepository>();
            builder.RegisterType<CommentLikeRepository>().As<ICommentLikeRepository>();
            builder.RegisterType<ReplyLikeRepository>().As<IReplyLikeRepository>();
            builder.RegisterType<ItemRepository>().As<IItemRepository>();
            builder.RegisterType<GamificationRepository>().As<IGamificationRepository>();
            builder.RegisterType<LibraryRepository>().As<ILibraryRepository>();
            builder.RegisterType<ChatUserRepository>().As<IChatUserRepository>();
            builder.RegisterType<ConnectionRepository>().As<IConnectionRepository>();
            builder.RegisterType<FlashcardPinRepository>().As<IFlashcardPinRepository>();
        }
    }
}
