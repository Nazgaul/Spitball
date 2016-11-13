using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;

namespace Zbang.Zbox.Domain.Services
{
    public class ZboxWriteService : IZboxWriteService, IZboxServiceBootStrapper
    {
        private readonly ICommandBus m_CommandBus;
        private readonly IWithCache m_Cache;

        public ZboxWriteService(ICommandBus commandBus, IWithCache cache)
        {
            m_CommandBus = commandBus;
            m_Cache = cache;
        }

        public void BootStrapper()
        {
            using (UnitOfWork.Start())
            { }
        }





        #region IZboxWriteOnlyService

        public async Task<CreateUserCommandResult> CreateUserAsync(CreateUserCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            using (UnitOfWork.Start())
            {
                using (UnitOfWork.Current.BeginTransaction())
                {
                    var result = await m_CommandBus.DispatchAsync<CreateUserCommand, CreateUserCommandResult>(command, command.CommandResolveName);
                    if (command.BoxId.HasValue)
                    {
                        var autoFollowCommand = new SubscribeToSharedBoxCommand(result.User.Id, command.BoxId.Value);
                        await m_CommandBus.SendAsync(autoFollowCommand);
                    }
                    UnitOfWork.Current.TransactionalFlush();
                    return result;
                }
            }
        }

        public async Task<UpdateUserCommandResult> UpdateUserPasswordAsync(UpdateUserPasswordCommand command)
        {
            using (UnitOfWork.Start())
            {
                var result = await m_CommandBus.DispatchAsync<UpdateUserPasswordCommand, UpdateUserCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();
                return result;
            }
        }

        public async Task UpdateUserEmailAsync(UpdateUserEmailCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void UpdateUserProfile(UpdateUserProfileCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void UpdateUserImage(UpdateUserProfileImageCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task<CreateBoxCommandResult> CreateBoxAsync(CreateBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            using (UnitOfWork.Start())
            {
                var commandCache = command as ICommandCache;
                if (commandCache != null)
                {
                    await m_Cache.RemoveAsync(commandCache);
                }
                var result = m_CommandBus.Dispatch<CreateBoxCommand, CreateBoxCommandResult>(command, command.GetType().Name);
                UnitOfWork.Current.TransactionalFlush();
                return result;
            }
        }

        public void ChangeBoxInfo(ChangeBoxInfoCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


        public void ChangeNotificationSettings(ChangeNotificationSettingsCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


        public async Task UnFollowBoxAsync(UnFollowBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task<AddItemToBoxCommandResult> AddItemToBoxAsync(AddItemToBoxCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                var reputationCommand = new AddReputationCommand(command.UserId,
                     Infrastructure.Enums.ReputationAction.AddItem);
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                var t4 = m_Cache.RemoveAsync(command);
                var t3 = m_CommandBus.SendAsync(autoFollowCommand);
                var t2 = m_CommandBus.DispatchAsync<AddItemToBoxCommand, AddItemToBoxCommandResult>(command, command.ResolverName);
                var t1 = m_CommandBus.SendAsync(reputationCommand);

                await Task.WhenAll(t1, t2, t3, t4);
                unitOfWork.TransactionalFlush();

                return t2.Result;
            }
        }

        public async Task DeleteItemAsync(DeleteItemCommand command)
        {
            using (UnitOfWork.Start())
            {
                var t1 = m_CommandBus.SendAsync(command);
                var t2 = m_Cache.RemoveAsync(command);
                await Task.WhenAll(t1, t2);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task RateItemAsync(RateItemCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                var t1 = m_CommandBus.SendAsync(autoFollowCommand);
                var t2 = m_CommandBus.SendAsync(command);
                await Task.WhenAll(t1, t2);

                UnitOfWork.Current.TransactionalFlush();
            }
        }




        public void DeleteUserFromBox(DeleteUserFromBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        //public async Task SendMessageAsync(SendMessageCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        await m_CommandBus.SendAsync(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}

        public async Task ShareBoxAsync(ShareBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.InviteeId, command.BoxId);
                var t1 = m_CommandBus.SendAsync(autoFollowCommand);
                var t2 = m_CommandBus.SendAsync(command);
                await Task.WhenAll(t1, t2);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task InviteSystemAsync(InviteToSystemCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


        public async Task SubscribeToSharedBoxAsync(SubscribeToSharedBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }





        public ChangeFileNameCommandResult ChangeFileName(ChangeFileNameCommand command)
        {
            using (UnitOfWork.Start())
            {
                ChangeFileNameCommandResult result = m_CommandBus.Dispatch<ChangeFileNameCommand, ChangeFileNameCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();
                return result;
            }
        }




        public void UpdateUserLanguage(UpdateUserLanguageCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void UpdateUserEmailSettings(UpdateUserEmailSubscribeCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


        public void UpdateNodeSettings(UpdateNodeSettingsCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void UpdatePreviewFailed(PreviewFailedCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task DeleteNodeLibraryAsync(DeleteNodeFromLibraryCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                await m_Cache.RemoveAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void UpdateUserUniversity(UpdateUserUniversityCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task CreateBoxItemTabAsync(CreateItemTabCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                await m_CommandBus.SendAsync(autoFollowCommand);
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task AssignBoxItemToTabAsync(AssignItemToTabCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                await m_CommandBus.SendAsync(autoFollowCommand);
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void RenameBoxItemTab(ChangeItemTabNameCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void DeleteBoxItemTab(DeleteItemTabCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }




        #region annotation
        public async Task AddAnnotationAsync(AddItemCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                var t1 = m_CommandBus.SendAsync(autoFollowCommand);
                var t3 = m_Cache.RemoveAsync(command);
                var t2 = m_CommandBus.SendAsync(command);
                await Task.WhenAll(t1, t2, t3);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task AddReplyAnnotationAsync(AddItemReplyToCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                var t1 = m_CommandBus.SendAsync(command);
                var t2 = m_CommandBus.SendAsync(autoFollowCommand);
                var t3 = m_Cache.RemoveAsync(command);
                await Task.WhenAll(t1, t2, t3);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task DeleteAnnotationAsync(DeleteItemCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                var t1 = m_CommandBus.SendAsync(command);
                var t2 = m_Cache.RemoveAsync(command);
                await Task.WhenAll(t1, t2);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task DeleteItemCommentReplyAsync(DeleteItemCommentReplyCommand command)
        {
            using (UnitOfWork.Start())
            {
                var t1 = m_CommandBus.SendAsync(command);
                var t2 = m_Cache.RemoveAsync(command);
                await Task.WhenAll(t1, t2);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        #endregion

        #region QnA
        public async Task<AddCommentCommandResult> AddCommentAsync(AddCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                var reputationCommand = new AddReputationCommand(command.UserId,
                     Infrastructure.Enums.ReputationAction.AddComment);
                var t4 = m_Cache.RemoveAsync(command);
                var t3 = m_CommandBus.SendAsync(reputationCommand);
                var t2 = m_CommandBus.SendAsync(autoFollowCommand);
                var t1 = m_CommandBus.DispatchAsync<AddCommentCommand, AddCommentCommandResult>(command);


                await Task.WhenAll(t1, t2, t3, t4);
                UnitOfWork.Current.TransactionalFlush();
                return t1.Result;
            }
        }
        public async Task AddReplyAsync(AddReplyToCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                var reputationCommand = new AddReputationCommand(command.UserId,
                     Infrastructure.Enums.ReputationAction.AddReply);
                var t4 = m_Cache.RemoveAsync(command);
                var t3 = m_CommandBus.SendAsync(reputationCommand);
                var t2 = m_CommandBus.SendAsync(autoFollowCommand);
                var t1 = m_CommandBus.SendAsync(command);


                await Task.WhenAll(t1, t2, t3, t4);
                UnitOfWork.Current.TransactionalFlush();

            }
        }

        public async Task DeleteCommentAsync(DeleteCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                var t2 = m_Cache.RemoveAsync(command);
                var t1 = m_CommandBus.SendAsync(command);

                await Task.WhenAll(t1, t2);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task DeleteReplyAsync(DeleteReplyCommand command)
        {
            using (UnitOfWork.Start())
            {
                var t1 = m_CommandBus.SendAsync(command);
                var t2 = m_Cache.RemoveAsync(command);
                await Task.WhenAll(t1, t2);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task<LikeCommentCommandResult> LikeCommentAsync(LikeCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                var t2 = m_Cache.RemoveAsync(command);
                var t1 = m_CommandBus.SendAsync(autoFollowCommand);

                await Task.WhenAll(t1, t2);
                var result = m_CommandBus.Dispatch<LikeCommentCommand, LikeCommentCommandResult>(command);

                UnitOfWork.Current.TransactionalFlush();
                return result;
            }
        }
        public async Task<LikeReplyCommandResult> LikeReplyAsync(LikeReplyCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                var t2 = m_Cache.RemoveAsync(command);
                var t1 = m_CommandBus.SendAsync(autoFollowCommand);

                await Task.WhenAll(t1, t2);
                var result = m_CommandBus.Dispatch<LikeReplyCommand, LikeReplyCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();
                return result;
            }
        }
        #endregion

        /// <summary>
        /// Create new university via windows app
        /// </summary>
        /// <param name="command"></param>
        public async Task CreateUniversityAsync(CreateUniversityCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }



        #endregion


        public async Task AddReputationAsync(AddReputationCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }



        public void DeleteUpdates(DeleteUpdatesCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        #region quiz
        public async Task CreateQuizAsync(CreateQuizCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);


                m_CommandBus.Send(command);
                await m_CommandBus.SendAsync(autoFollowCommand);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void UpdateQuiz(UpdateQuizCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task DeleteQuizAsync(DeleteQuizCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void CreateQuestion(CreateQuestionCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        //public void UpdateQuestion(UpdateQuestionCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}
        public void DeleteQuestion(DeleteQuestionCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        //public void CreateAnswer(CreateAnswerCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}
        //public void DeleteAnswer(DeleteAnswerCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}
        //public void UpdateAnswer(UpdateAnswerCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}
        //public void MarkAnswerAsCorrect(MarkAnswerCorrectCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}
        public async Task<SaveQuizCommandResult> SaveQuizAsync(SaveQuizCommand command)
        {
            using (UnitOfWork.Start())
            {
                var reputationCommand = new AddReputationCommand(command.UserId,
                     Infrastructure.Enums.ReputationAction.AddQuiz);

                var t1 = m_CommandBus.SendAsync(reputationCommand);
                var t2 = m_CommandBus.DispatchAsync<SaveQuizCommand, SaveQuizCommandResult>(command);
                await Task.WhenAll(t1, t2);
                UnitOfWork.Current.TransactionalFlush();
                return t2.Result;
            }
        }
        public async Task SaveUserAnswersAsync(SaveUserQuizCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.UserId, command.BoxId);
                await m_CommandBus.SendAsync(autoFollowCommand);
                m_CommandBus.Send(command);


                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task CreateItemInDiscussionAsync(CreateDiscussionCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void DeleteItemInDiscussion(DeleteDiscussionCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        #endregion
        public void AddStudent(AddStudentCommand command)
        {

            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task CreateDepartmentAsync(AddNodeToLibraryCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                await m_Cache.RemoveAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task RequestAccessToDepartmentAsync(RequestAccessLibraryNodeCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task RequestAccessToDepartmentApprovedAsync(LibraryNodeApproveAccessCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void RegisterMobileDevice(RegisterMobileDeviceCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void ChangeOnlineStatus(ChangeUserOnlineStatusCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task AddChatMessageAsync(ChatAddMessageCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void MarkChatAsRead(ChatMarkAsReadCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void ManageConnections(ManageConnectionsCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void RemoveOldConnections(RemoveOldConnectionCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task AddUserLocationActivityAsync(AddUserLocationActivityCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task AddFlashcardAsync(AddFlashcardCommand command)
        {
            using (UnitOfWork.Start())
            {
                var autoFollowCommand = new SubscribeToSharedBoxCommand(command.Flashcard.UserId, command.Flashcard.BoxId);
                var t1 = m_CommandBus.SendAsync(autoFollowCommand);
                var t2 = m_CommandBus.SendAsync(command);
                await Task.WhenAll(t1, t2);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task UpdateFlashcardAsync(UpdateFlashcardCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task PublishFlashcardAsync(PublishFlashcardCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

    }
}
