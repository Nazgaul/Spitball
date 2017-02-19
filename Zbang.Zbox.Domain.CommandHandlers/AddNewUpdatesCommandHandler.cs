using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddNewUpdatesCommandHandler : ICommandHandlerAsync<AddNewUpdatesCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<CommentReply> m_ReplyRepository;
        private readonly IRepository<Comment> m_CommentRepository;
        private readonly IRepository<Updates> m_UpdatesRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IRepository<ItemCommentReply> m_ItemCommentReplyRepository;
        private readonly IRepository<QuizDiscussion> m_QuizDiscussionRepository;
        private readonly ISendPush m_SendPush;
        private readonly IQueueProvider m_QueueProvider;

        //private readonly IMailComponent m_MailComponent;

        public AddNewUpdatesCommandHandler(
            IBoxRepository boxRepository,
            IRepository<Item> itemRepository,
            IRepository<CommentReply> replyRepository,
            IRepository<Comment> commentRepository,
            IRepository<Updates> updatesRepository,
            IRepository<Domain.Quiz> quizRepository, ISendPush sendPush,
            IUserRepository userRepository, IRepository<ItemComment> itemCommentRepository,
            IRepository<ItemCommentReply> itemCommentReplyRepository, IRepository<QuizDiscussion> quizDiscussionRepository, IQueueProvider queueProvider)
        {
            m_BoxRepository = boxRepository;
            m_ItemRepository = itemRepository;
            m_ReplyRepository = replyRepository;
            m_CommentRepository = commentRepository;
            m_UpdatesRepository = updatesRepository;
            m_QuizRepository = quizRepository;
            m_SendPush = sendPush;
            m_UserRepository = userRepository;
            m_ItemCommentRepository = itemCommentRepository;
            m_ItemCommentReplyRepository = itemCommentReplyRepository;
            m_QuizDiscussionRepository = quizDiscussionRepository;
            m_QueueProvider = queueProvider;
        }
        public Task HandleAsync(AddNewUpdatesCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var box = m_BoxRepository.Load(message.BoxId);
            //there was an issue with commit with detach elements
            var usersToUpdate = box.UserBoxRelationship.Where(w => w.User.Id != message.UserId).Select(s => s.UserId).ToList();

            var tQuiz = UpdateQuizAsync(message.QuizId, usersToUpdate, box);
            var tItem = UpdateItemAsync(message.ItemId, usersToUpdate, box);
            var tComment = UpdateCommentAsync(message.CommentId, usersToUpdate, box);
            var tReply = UpdateReplyAsync(message.ReplyId, usersToUpdate, box);
            var tItemDiscussion = UpdateItemDiscussionAsync(message.ItemDiscussionId, usersToUpdate, box);
            var tItemDiscussionReply = UpdateItemReplyDiscussionAsync(message.ItemDiscussionReplyId, usersToUpdate, box);
            var tQuizDiscussion = UpdateQuizDiscussionAsync(message.QuizDiscussionId, usersToUpdate, box);


            return Task.WhenAll(tQuiz, tItem, tComment, tReply, tItemDiscussion, tItemDiscussionReply, tQuizDiscussion);
        }

        private void DoUpdateLoop(IEnumerable<long> userIds, Func<User, Updates> update)
        {
            foreach (var userId in userIds)
            {
                var newUpdate = update(m_UserRepository.Load(userId));
                m_UpdatesRepository.Save(newUpdate);
            }
        }

        private Task UpdateQuizAsync(long? quizId, IEnumerable<long> userIds, Box box)
        {
            if (!quizId.HasValue)
            {
                return Task.CompletedTask;
            }
            var quiz = m_QuizRepository.Load(quizId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, quiz));
            return Task.CompletedTask;
        }

        private Task UpdateItemAsync(long? itemId, IList<long> userIds, Box box)
        {
            if (!itemId.HasValue)
            {
                return Task.CompletedTask;
            }
            var item = m_ItemRepository.Load(itemId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, item));
            return m_SendPush.SendAddItemNotificationAsync(item.User.Name, box.Name, box.Id, userIds);
        }



        private Task UpdateReplyAsync(Guid? replyId, IList<long> userIds, Box box)
        {
            if (!replyId.HasValue)
            {
                return Task.CompletedTask;
            }
            var reply = m_ReplyRepository.Load(replyId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, reply));

            var t1 = Task.CompletedTask;
            var commentUser = reply.Question.User;
            if (commentUser.Id != reply.User.Id && commentUser.EmailSendSettings == Infrastructure.Enums.EmailSend.CanSend)
            {
                t1 = m_QueueProvider.InsertMessageToMailNewAsync(new ReplyToCommentData(commentUser.Email,
                     commentUser.Culture,
                     commentUser.Name,
                     reply.User.Name,
                     reply.Box.Name,
                     reply.Box.Url));
                //t1 = m_MailComponent.GenerateAndSendEmailAsync(reply.Question.User.Email,
                //new ReplyToCommentMailParams(new CultureInfo(reply.Question.User.Culture), reply.Question.User.Name, reply.User.Name, box.Name, box.Url));
            }
            var t2 = m_SendPush.SendAddReplyNotificationAsync(reply.User.Name, reply.Text, box.Name, box.Id, reply.Question.Id, userIds);
            return Task.WhenAll(t1, t2);
        }

        private Task UpdateCommentAsync(Guid? commentId, IList<long> userIds, Box box)
        {
            if (!commentId.HasValue)
            {
                return Task.CompletedTask;
            }
            var comment = m_CommentRepository.Load(commentId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, comment));
            if (string.IsNullOrEmpty(comment.Text))
            {
                return Task.CompletedTask;
            }
            try
            {
                var removeHtmlRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
                var textToPush = removeHtmlRegex.Replace(comment.Text, string.Empty);
                if (string.IsNullOrEmpty(textToPush))
                {
                    return Task.CompletedTask;
                }

                return m_SendPush.SendAddPostNotificationAsync(comment.User.Name, textToPush, box.Name, box.Id, userIds);
            }
            catch (ArgumentNullException ex)
            {
                TraceLog.WriteError("regex error text: " + comment.Text, ex);
                throw;
            }
        }

        private Task UpdateItemDiscussionAsync(long? itemDiscussionId, IEnumerable<long> userIds, Box box)
        {
            if (!itemDiscussionId.HasValue)
            {
                return Task.CompletedTask;
            }
            var itemDiscussion = m_ItemCommentRepository.Load(itemDiscussionId.Value);
            DoUpdateLoop(userIds, u => Updates.UpdateItemDiscussion(u, box, itemDiscussion));
            return Task.CompletedTask;
        }
        private Task UpdateItemReplyDiscussionAsync(long? itemReplyDiscussionId, IEnumerable<long> userIds, Box box)
        {
            if (!itemReplyDiscussionId.HasValue)
            {
                return Task.CompletedTask;
            }
            var itemReplyDiscussion = m_ItemCommentReplyRepository.Load(itemReplyDiscussionId.Value);
            DoUpdateLoop(userIds, u => Updates.UpdateItemDiscussionReply(u, box, itemReplyDiscussion));
            var userDiscussion = itemReplyDiscussion.Parent.Author;
            if (userDiscussion.Id == itemReplyDiscussion.Author.Id || userDiscussion.EmailSendSettings != Infrastructure.Enums.EmailSend.CanSend)
            {
                return Task.CompletedTask;
            }
            return m_QueueProvider.InsertMessageToMailNewAsync(new ReplyToCommentData(userDiscussion.Email,
                    userDiscussion.Culture,
                    userDiscussion.Name,
                    itemReplyDiscussion.Author.Name,
                    itemReplyDiscussion.Item.Name,
                    itemReplyDiscussion.Item.Url));
        }
        private Task UpdateQuizDiscussionAsync(Guid? quizDiscussionId, IEnumerable<long> userIds, Box box)
        {
            if (!quizDiscussionId.HasValue)
            {
                return Task.CompletedTask;
            }
            var quizDiscussion = m_QuizDiscussionRepository.Load(quizDiscussionId.Value);
            DoUpdateLoop(userIds, u => Updates.UpdateQuizDiscussion(u, box, quizDiscussion));
            return Task.CompletedTask;
        }

    }
}
