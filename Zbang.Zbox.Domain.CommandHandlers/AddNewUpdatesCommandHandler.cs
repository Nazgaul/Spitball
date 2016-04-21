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

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddNewUpdatesCommandHandler : ICommandHandlerAsync<AddNewUpdatesCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<CommentReplies> m_ReplyRepository;
        private readonly IRepository<Comment> m_CommentRepository;
        private readonly IRepository<Updates> m_UpdatesRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly ISendPush m_SendPush;

        public AddNewUpdatesCommandHandler(
            IBoxRepository boxRepository,
            IRepository<Item> itemRepository,
            IRepository<CommentReplies> replyRepository,
            IRepository<Comment> commentRepository,
            IRepository<Updates> updatesRepository,
            IRepository<Domain.Quiz> quizRepository, ISendPush sendPush, IUserRepository userRepository)
        {
            m_BoxRepository = boxRepository;
            m_ItemRepository = itemRepository;
            m_ReplyRepository = replyRepository;
            m_CommentRepository = commentRepository;
            m_UpdatesRepository = updatesRepository;
            m_QuizRepository = quizRepository;
            m_SendPush = sendPush;
            m_UserRepository = userRepository;
        }
        public Task HandleAsync(AddNewUpdatesCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var box = m_BoxRepository.Load(message.BoxId); 
            var usersToUpdate = box.UserBoxRelationship.Where(w => w.User.Id != message.UserId).Select(s => s.UserId).ToList();
            if (usersToUpdate.Count == 0)
            {
                return Task.FromResult<object>(null);
            }
            var tQuiz = UpdateQuiz(message.QuizId,usersToUpdate,box);
            var tItem = UpdateItem(message.ItemId, usersToUpdate, box);
            var tComment = UpdateComment(message.CommentId, usersToUpdate, box);
            var tReply = UpdateReply(message.ReplyId, usersToUpdate, box);

            return Task.WhenAll(tQuiz, tItem, tComment, tReply);
        }

        private void DoUpdateLoop(IEnumerable<long> userIds, Func<User,Updates> update)
        {
            foreach (var userId in userIds)
            {
                var user = m_UserRepository.Load(userId);
                var newUpdate = update(user);
                m_UpdatesRepository.Save(newUpdate);
            }
        }

        private Task UpdateQuiz(long? quizId , IEnumerable<long> userIds, Box box)
        {
            if (!quizId.HasValue)
            {
                return Task.FromResult<object>(null);
            }
            var quiz =  m_QuizRepository.Load(quizId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, quiz));
            return Task.FromResult<object>(null);
        }

        private Task UpdateItem(long? itemId, IList<long> userIds, Box box)
        {
            if (!itemId.HasValue)
            {
                return Task.FromResult<object>(null);
            }
            var item = m_ItemRepository.Load(itemId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, item));
            //if (box.Actual is AcademicBoxClosed)
            //{
            //    return Task.FromResult<object>(null);
            //}
           return m_SendPush.SendAddItemNotification(item.Uploader.Name, box.Name, box.Id, userIds);
                        
          
        }



        private Task UpdateReply(Guid? replyId, IList<long> userIds, Box box)
        {
            if (!replyId.HasValue)
            {
                return Task.FromResult<object>(null);
            }
             var reply =  m_ReplyRepository.Load(replyId.Value);
             DoUpdateLoop(userIds, u => new Updates(u, box, reply));
             //if (box.Actual is AcademicBoxClosed)
             //{
             //    return Task.FromResult<object>(null);
             //}
             return m_SendPush.SendAddReplyNotification(reply.User.Name, reply.Text, box.Name, box.Id, reply.Question.Id, userIds);
        }

        private Task UpdateComment(Guid? commentId, IList<long> userIds, Box box)
        {
            if (!commentId.HasValue)
            {
                return Task.FromResult<object>(null);
            }
            var comment =  m_CommentRepository.Load(commentId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, comment));
            if (string.IsNullOrEmpty(comment.Text))
            {
                Task.FromResult<object>(null);
            }
            //if (box.Actual is AcademicBoxClosed)
            //{
            //    return Task.FromResult<object>(null);
            //}
            try
            {
                var removeHtmlRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
                var textToPush = removeHtmlRegex.Replace(comment.Text, string.Empty);
                if (string.IsNullOrEmpty(textToPush))
                {
                    Task.FromResult<object>(null);
                }

                return m_SendPush.SendAddPostNotification(comment.User.Name, textToPush, box.Name, box.Id, userIds);
            }
            catch (ArgumentNullException ex)
            {
                TraceLog.WriteError("regex error text: " + comment.Text, ex);
                throw;
            }
        }

    }
}
