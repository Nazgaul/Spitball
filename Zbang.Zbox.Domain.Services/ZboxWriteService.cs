using NHibernate;
using NHibernate.Criterion;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Zbox.Domain.Services
{
    public partial class ZboxWriteService : IZboxWriteService, IZboxServiceBootStrapper, IZboxServiceBackgroundApp
    {
        private readonly ICommandBus m_CommandBus;

        public ZboxWriteService(ICommandBus commandBus)
        {
            m_CommandBus = commandBus;
        }

        public void BootStrapper()
        {
            using (UnitOfWork.Start())
            { }
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
                        UnitOfWork.CurrentSession.Save(box);
                    }
                    tx.Commit();
                }
            }
        }

        #region IZboxWriteOnlyService

        public CreateUserCommandResult CreateUser(CreateUserCommand command)
        {

            using (UnitOfWork.Start())
            {
                using (UnitOfWork.Current.BeginTransaction())
                {
                    CreateUserCommandResult result = m_CommandBus.Dispatch<CreateUserCommand, CreateUserCommandResult>(command, command.CommandResolveName);
                    UnitOfWork.Current.TransactionalFlush();
                    return result;
                }
            }
        }

        public UpdateUserCommandResult UpdateUserPassword(UpdateUserPasswordCommand command)
        {
            using (UnitOfWork.Start())
            {
                UpdateUserCommandResult result = m_CommandBus.Dispatch<UpdateUserPasswordCommand, UpdateUserCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();
                return result;
            }
        }

        public void UpdateUserEmail(UpdateUserEmailCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
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

        public CreateBoxCommandResult CreateBox(CreateBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                CreateBoxCommandResult result = m_CommandBus.Dispatch<CreateBoxCommand, CreateBoxCommandResult>(command, command.GetType().Name);
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

        public ChangeBoxPrivacySettingsCommandResult ChangeBoxPrivacySettings(ChangeBoxPrivacySettingsCommand command)
        {
            using (UnitOfWork.Start())
            {
                ChangeBoxPrivacySettingsCommandResult result = m_CommandBus.Dispatch<ChangeBoxPrivacySettingsCommand, ChangeBoxPrivacySettingsCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();
                return result;
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

        public void DeleteBox(DeleteBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void UnfollowBox(UnfollowBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public AddFileToBoxCommandResult AddFileToBox(AddFileToBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                AddFileToBoxCommandResult result = m_CommandBus.Dispatch<AddFileToBoxCommand, AddFileToBoxCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();

                return result;
            }
        }

        public AddLinkToBoxCommandResult AddLinkToBox(AddLinkToBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                AddLinkToBoxCommandResult result = m_CommandBus.Dispatch<AddLinkToBoxCommand, AddLinkToBoxCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();
                return result;
            }
        }

        public AddBoxCommentCommandResult AddBoxComment(AddBoxCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                AddBoxCommentCommandResult result = m_CommandBus.Dispatch<AddBoxCommentCommand, AddBoxCommentCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();

                return result;
            }
        }

        //public AddItemCommentCommandResult AddItemComment(AddItemCommentCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        AddItemCommentCommandResult result = m_CommandBus.Dispatch<AddItemCommentCommand, AddItemCommentCommandResult>(command);
        //        UnitOfWork.Current.TransactionalFlush();

        //        return result;
        //    }
        //}

        public AddReplyToCommentCommandResult AddReplyToComment(AddReplyToCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                AddReplyToCommentCommandResult result = m_CommandBus.Dispatch<AddReplyToCommentCommand, AddReplyToCommentCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();

                return result;
            }
        }

        public void DeleteItem(DeleteItemCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void RateItem(RateItemCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        //public void FlagBadItem(FlagItemAsBadCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}

        //public void LikeItem(LikeItemCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}
        public void Statistics(UpdateStatisticsCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
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

        public void SendMessage(SendMessageCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void ShareBox(ShareBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void InviteSystem(InviteToSystemCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void ShareBoxFacebbok(ShareBoxFacebookCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void SubscribeToSharedBox(SubscribeToSharedBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void InviteSystemFromFacebook(InviteToSystemFacebookCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        //public void VerifyEmail(VerifyEmailCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send<VerifyEmailCommand>(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}


        public void DeleteComment(DeleteCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
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


        //public void AddTagToBox(AddTagToBoxCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send<AddTagToBoxCommand>(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}
        //public void DeleteTagFromBox(DeleteTagFromBoxCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send<DeleteTagFromBoxCommand>(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}

        //public void AddAFriend(AddFriendCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}

        //public void ChangeBoxNotification(ChangeBoxNotificationSettingsCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}

        public void UpdateUserLanguage(UpdateUserLanguageCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }


        public void AddNodeToLibrary(AddNodeToLibraryCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void RenameNodeLibrary(RenameNodeCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void DeleteNodeLibrary(DeleteNodeFromLibraryCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
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

        public void CreateBoxItemTab(CreateItemTabCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void AssignBoxItemToTab(AssignItemToTabCommand command)
        {
            using (UnitOfWork.Start())
            {
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


        public void UpdateUserFirstTimeStatus(UpdateUserFirstTimeStatusCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        #region annotation
        public void AddAnnotation(AddAnnotationCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void AddReplyAnnotation(AddReplyToAnnotationCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void DeleteAnnotation(DeleteAnnotationCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        #endregion

        #region QnA
        public void AddQuestion(AddQuestionCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task AddAnswer(AddAnswerToQuestionCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
                UnitOfWork.Current.TransactionalFlush();
                
            }
        }
        public void MarkCorrectAnswer(MarkAsAnswerCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void RateAnswer(RateAnswerCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void DeleteFileFromQnA(DeleteFileFromQnACommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void DeleteQuestion(DeleteQuestionCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void DeleteAnswer(DeleteAnswerCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        #endregion

        /// <summary>
        /// Create new university via windows app
        /// </summary>
        /// <param name="command"></param>
        public void CreateUniversity(CreateUniversityCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }



        #endregion


        public void AddReputation(AddReputationCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public void AddNewUpdate(AddNewUpdatesCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
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

        public void MarkMessageAsRead(MarkMessagesAsReadCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void MarkMessagesAsOld(MarkMessagesAsOldCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
    }
}
