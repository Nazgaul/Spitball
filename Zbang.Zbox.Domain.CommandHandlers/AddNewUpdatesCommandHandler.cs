using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddNewUpdatesCommandHandler : ICommandHandlerAsync<AddNewUpdatesCommand>
    {
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<CommentReplies> m_AnswerRepository;
        private readonly IRepository<Comment> m_QuestionRepository;
        private readonly IRepository<Updates> m_UpdatesRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly ISendPush m_SendPush;

        public AddNewUpdatesCommandHandler(
            IBoxRepository boxRepository,
            IRepository<Item> itemRepository,
            IRepository<CommentReplies> answerRepository,
            IRepository<Comment> questionRepository,
            IRepository<Updates> updatesRepository,
            IRepository<Domain.Quiz> quizRepository, ISendPush sendPush)
        {
            m_BoxRepository = boxRepository;
            m_ItemRepository = itemRepository;
            m_AnswerRepository = answerRepository;
            m_QuestionRepository = questionRepository;
            m_UpdatesRepository = updatesRepository;
            m_QuizRepository = quizRepository;
            m_SendPush = sendPush;
        }
        public Task HandleAsync(AddNewUpdatesCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var box = m_BoxRepository.Load(message.BoxId);
            var usersToUpdate = box.UserBoxRelationship.Where(w => w.User.Id != message.UserId).ToList();

            var quiz = GetQuiz(message.QuizId);
            var item = GetItem(message.ItemId);
            var reply = GetReply(message.ReplyId);
            var comment = GetComment(message.CommentId);

            foreach (var userBoxRel in usersToUpdate)
            {
                var newUpdate = new Updates(userBoxRel.User, box,
                    comment, reply, item, quiz);
                m_UpdatesRepository.Save(newUpdate);
            }
            if (usersToUpdate.Count > 0)
            {
                if (item != null)
                {
                    return m_SendPush.SendAddItemNotification(item.Uploader.Name, box.Name,
                        usersToUpdate.Select(s => s.UserId).ToList());
                }
                if (comment != null)
                {
                    return m_SendPush.SendAddPostNotification(comment.User.Name, comment.Text, box.Name,
                        usersToUpdate.Select(s => s.UserId).ToList());
                }
                if (reply != null)
                {
                    return m_SendPush.SendAddReplyNotification(reply.User.Name, reply.Text, box.Name,
                        usersToUpdate.Select(s => s.UserId).ToList());
                }
            }
            return Task.FromResult(true);

        }
        private Domain.Quiz GetQuiz(long? quizId)
        {
            if (!quizId.HasValue)
            {
                return null;
            }
            return m_QuizRepository.Load(quizId.Value);
        }

        private Item GetItem(long? itemId)
        {
            if (!itemId.HasValue)
            {
                return null;
            }
            return m_ItemRepository.Load(itemId.Value);
        }

        private CommentReplies GetReply(Guid? replyId)
        {
            if (!replyId.HasValue)
            {
                return null;
            }
            return m_AnswerRepository.Load(replyId.Value);
        }

        private Comment GetComment(Guid? commentId)
        {
            if (!commentId.HasValue)
            {
                return null;
            }
            return m_QuestionRepository.Load(commentId.Value);
        }

    }
}
