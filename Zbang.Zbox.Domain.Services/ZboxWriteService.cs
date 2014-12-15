﻿using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;

namespace Zbang.Zbox.Domain.Services
{
    public partial class ZboxWriteService : IZboxWriteService, IZboxServiceBootStrapper
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





        #region IZboxWriteOnlyService

        public CreateUserCommandResult CreateUser(CreateUserCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

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
            if (command == null) throw new ArgumentNullException("command");
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


        public void ChangeNotificationSettings(ChangeNotificationSettingsCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        //public void DeleteBox(DeleteBoxCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}
        public void UnFollowBox(UnfollowBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }

        public async Task<AddFileToBoxCommandResult> AddFileToBox(AddFileToBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                var reputationCommand = new AddReputationCommand(command.UserId,
                    Infrastructure.Enums.ReputationAction.AddItem);

                var t1 = m_CommandBus.SendAsync(reputationCommand);
                var t2 = m_CommandBus.DispatchAsync<AddFileToBoxCommand, AddFileToBoxCommandResult>(command);
                await Task.WhenAll(t1, t2);
                UnitOfWork.Current.TransactionalFlush();

                return t2.Result;
            }
        }

        public async Task<AddLinkToBoxCommandResult> AddLinkToBox(AddLinkToBoxCommand command)
        {
            using (UnitOfWork.Start())
            {
                AddLinkToBoxCommandResult result = await m_CommandBus.DispatchAsync<AddLinkToBoxCommand, AddLinkToBoxCommandResult>(command);
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
        public void ShareBoxFacebook(ShareBoxFacebookCommand command)
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


        //public void AddNodeToLibrary(AddNodeToLibraryCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}

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
        public void DeleteAnnotation(DeleteItemCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void DeleteItemCommentReply(DeleteItemCommentReplyCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        #endregion

        #region QnA
        public async Task AddQuestion(AddCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
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

        public void DeleteComment(DeleteCommentCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void DeleteAnswer(DeleteReplyCommand command)
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


        public async Task AddReputation(AddReputationCommand command)
        {
            using (UnitOfWork.Start())
            {
                await m_CommandBus.SendAsync(command);
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



        //public void MarkMessagesAsOld(MarkMessagesAsOldCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}

        #region quiz
        public void CreateQuiz(CreateQuizCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
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
        public void DeleteQuiz(DeleteQuizCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
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
        public void UpdateQuestion(UpdateQuestionCommand command)
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
        public void CreateAnswer(CreateAnswerCommand command)
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
        public void UpdateAnswer(UpdateAnswerCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void MarkAnswerAsCorrect(MarkAnswerCorrectCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public async Task<SaveQuizCommandResult> SaveQuiz(SaveQuizCommand command)
        {
            using (UnitOfWork.Start())
            {
                var result = await m_CommandBus.DispatchAsync<SaveQuizCommand, SaveQuizCommandResult>(command);
                UnitOfWork.Current.TransactionalFlush();
                return result;
            }
        }
        public void SaveUserAnswers(SaveUserQuizCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        public void CreateItemInDiscussion(CreateDiscussionCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
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

        public void CreateDepartment(AddNodeToLibraryCommand command)
        {
            using (UnitOfWork.Start())
            {
                m_CommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
        //public void SelectDepartment(SelectDepartmentCommand command)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        m_CommandBus.Send(command);
        //        UnitOfWork.Current.TransactionalFlush();
        //    }
        //}


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
