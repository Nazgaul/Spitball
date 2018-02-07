using Autofac;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CommandsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();
            builder.RegisterType(typeof(ICommandHandler<UpdateUserProfileCommand>),
                typeof(UpdateUserProfileCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateUserProfileImageCommand>),
                typeof(UpdateUserProfileImageCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<UnFollowBoxCommand>), typeof(UnFollowBoxCommandHandler));
            builder.RegisterType(typeof(ICommandHandlerAsync<DeleteBoxCommand>), typeof(DeleteBoxCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateThumbnailCommand>), typeof(UpdateThumbnailCommandHandler));

            builder.RegisterType(typeof(ICommandHandlerAsync<SubscribeToSharedBoxCommand>), typeof(SubscribeToSharedBoxCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateUserLanguageCommand>), typeof(UpdateUserLanguageCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateUserEmailSubscribeCommand>), typeof(UpdateUserEmailSubscribeCommandHandler));

            builder.RegisterType(typeof(ICommandHandler<UpdateNodeSettingsCommand>), typeof(UpdateNodeSettingsCommandHandler));

            builder.RegisterType(typeof(ICommandHandler<UpdateUserUniversityCommand>), typeof(UpdateUserUniversityCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UnsubscribeUsersFromEmailCommand>), typeof(UnsubscribeUsersFromEmailCommandHandler));
            //statistics
            builder.RegisterType(typeof(ICommandHandlerAsync<UpdateStatisticsCommand>), typeof(UpdateStatisticsCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateBadgesCommand>), typeof(UpdateBadgesCommandHandler));
            builder.RegisterType(typeof(ICommandHandler<UpdateReputationCommand>), typeof(UpdateReputationCommandHandler));

            //create university app
            builder.RegisterType(typeof(ICommandHandler<UpdateUniversityStatsCommand>), typeof(UpdateUniversityStatsCommandHandler));
            //message
            builder.RegisterType(typeof(ICommandHandlerAsync<ShareBoxCommand>), typeof(ShareBoxCommandHandler));

            // builder.RegisterType(typeof(ICommandHandlerAsync<AddReputationCommand>), typeof(AddReputationCommandHandler));

            //updates
            builder.RegisterType(typeof(ICommandHandlerAsync<AddNewUpdatesCommand>), typeof(AddNewUpdatesCommandHandler));
            //library
            builder.RegisterType(typeof(ICommandHandlerAsync<RequestAccessLibraryNodeCommand>),
                typeof(RequestAccessLibraryNodeCommandHandler));

            builder.RegisterType(typeof(ICommandHandler<RemoveOldConnectionCommand>),
                typeof(RemoveOldConnectionCommandHandler));
        }
    }
}
