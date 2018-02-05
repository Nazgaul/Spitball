using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddNewUpdatesCommandHandler : ICommandHandlerAsync<AddNewUpdatesCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBoxRepository _boxRepository;
        private readonly Infrastructure.Repositories.IRepository<Item> _itemRepository;
        private readonly Infrastructure.Repositories.IRepository<CommentReply> _replyRepository;
        private readonly Infrastructure.Repositories.IRepository<Comment> _commentRepository;
        private readonly Infrastructure.Repositories.IRepository<Updates> _updatesRepository;
        private readonly Infrastructure.Repositories.IRepository<Quiz> _quizRepository;
        private readonly Infrastructure.Repositories.IRepository<ItemComment> _itemCommentRepository;
        private readonly Infrastructure.Repositories.IRepository<ItemCommentReply> _itemCommentReplyRepository;
        private readonly Infrastructure.Repositories.IRepository<QuizDiscussion> _quizDiscussionRepository;
        private readonly IQueueProvider _queueProvider;


        public AddNewUpdatesCommandHandler(
            IBoxRepository boxRepository, Infrastructure.Repositories.IRepository<Item> itemRepository, Infrastructure.Repositories.IRepository<CommentReply> replyRepository, Infrastructure.Repositories.IRepository<Comment> commentRepository, Infrastructure.Repositories.IRepository<Updates> updatesRepository, Infrastructure.Repositories.IRepository<Quiz> quizRepository,
            IUserRepository userRepository, Infrastructure.Repositories.IRepository<ItemComment> itemCommentRepository, Infrastructure.Repositories.IRepository<ItemCommentReply> itemCommentReplyRepository, Infrastructure.Repositories.IRepository<QuizDiscussion> quizDiscussionRepository,
            IQueueProvider queueProvider)
        {
            _boxRepository = boxRepository;
            _itemRepository = itemRepository;
            _replyRepository = replyRepository;
            _commentRepository = commentRepository;
            _updatesRepository = updatesRepository;
            _quizRepository = quizRepository;
            _userRepository = userRepository;
            _itemCommentRepository = itemCommentRepository;
            _itemCommentReplyRepository = itemCommentReplyRepository;
            _quizDiscussionRepository = quizDiscussionRepository;
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(AddNewUpdatesCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var usersToUpdate = _userRepository.GetUsersToUpdate(message.BoxId, message.UserId).ToList();
            var box = _boxRepository.Load(message.BoxId);

            var tQuiz = UpdateQuizAsync(message.QuizId, usersToUpdate, box);
            UpdateItem(message.ItemId, usersToUpdate, box);
            UpdateComment(message.CommentId, usersToUpdate, box);
            var tReply = UpdateReplyAsync(message.ReplyId, usersToUpdate, box);
            var tItemDiscussion = UpdateItemDiscussionAsync(message.ItemDiscussionId, usersToUpdate, box);
            var tItemDiscussionReply = UpdateItemReplyDiscussionAsync(message.ItemDiscussionReplyId, usersToUpdate, box);
            var tQuizDiscussion = UpdateQuizDiscussionAsync(message.QuizDiscussionId, usersToUpdate, box);
            //var tFlashcard = UpdateFlashcardAsync(message.FlashcardId, box);

            return Task.WhenAll(tQuiz, tReply, tItemDiscussion, tItemDiscussionReply, tQuizDiscussion);
        }

        //private Task UpdateFlashcardAsync(long? flashcardId, Box box)
        //{
        //    //if (!flashcardId.HasValue)
        //    //{
        //    return Task.CompletedTask;
        //    //}
        //    var flashcard = _flashcardRepository.Load(flashcardId.Value);
        //    DoUpdateLoop(userIds, u => new Updates(u, box, flashcard));
        //    //return m_JaredPush.SendItemPushAsync(flashcard.User.Name, box.Id, flashcard.Id, BuildBoxTag(box.Id),
        //    //Infrastructure.Enums.ItemType.Flashcard);
        //}

        private void DoUpdateLoop(IEnumerable<long> userIds, Func<User, Updates> update)
        {
            if (update == null) throw new ArgumentNullException(nameof(update));
            foreach (var userId in userIds)
            {
                var newUpdate = update(_userRepository.Load(userId));
                _updatesRepository.Save(newUpdate);
            }
        }

        private Task UpdateQuizAsync(long? quizId, IEnumerable<long> userIds, Box box)
        {
            if (!quizId.HasValue)
            {
                return Task.CompletedTask;
            }
            var quiz = _quizRepository.Load(quizId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, quiz));
            return Task.CompletedTask;
            //return m_JaredPush.SendItemPushAsync(quiz.User.Name, box.Id, quiz.Id, BuildBoxTag(box.Id),
            //Infrastructure.Enums.ItemType.Quiz);
        }

        private void UpdateItem(long? itemId, IList<long> userIds, Box box)
        {
            if (!itemId.HasValue)
            {
                return;
            }
            var item = _itemRepository.Load(itemId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, item));
           // var t1 = m_JaredPush.SendItemPushAsync(item.User.Name, item.BoxId, item.Id, BuildBoxTag(item.BoxId),
               // Infrastructure.Enums.ItemType.Document);
            //return _sendPush.SendAddItemNotificationAsync(item.User.Name, box.Name, box.Id, userIds);
        }

        private Task UpdateReplyAsync(Guid? replyId, IList<long> userIds, Box box)
        {
            if (!replyId.HasValue)
            {
                return Task.CompletedTask;
            }
            var reply = _replyRepository.Load(replyId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, reply));

            var t1 = Task.CompletedTask;
            var commentUser = reply.Question.User;
            if (commentUser.Id != reply.User.Id
                && commentUser.EmailSendSettings == Infrastructure.Enums.EmailSend.CanSend)
            {
                t1 = _queueProvider.InsertMessageToMailNewAsync(new ReplyToCommentData(commentUser.Email,
                   commentUser.Culture,
                   commentUser.Name,
                   reply.User.Name,
                   reply.Box.Name,
                   reply.Box.Url));
            }
            //var t2 = _sendPush.SendAddReplyNotificationAsync(reply.User.Name, reply.Text, box.Name, box.Id, reply.Question.Id, userIds);
            return t1;
        }

        private void UpdateComment(Guid? commentId, IEnumerable<long> userIds, Box box)
        {
            if (!commentId.HasValue)
            {
                return;
            }
            var comment = _commentRepository.Load(commentId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, comment));
            //if (string.IsNullOrEmpty(comment.Text))
            //{
            //    return;
            //}
            //try
            //{
            //    var removeHtmlRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
            //    var textToPush = removeHtmlRegex.Replace(comment.Text, string.Empty);
            //    if (string.IsNullOrEmpty(textToPush))
            //    {
            //        return;
            //    }

            //    return;
            //    //return _sendPush.SendAddPostNotificationAsync(comment.User.Name, textToPush, box.Name, box.Id, userIds);
            //}
            //catch (ArgumentNullException ex)
            //{
            //    _logger.Exception(ex, new Dictionary<string, string>
            //    {
            //        ["source"] = "addNewUpdates",
            //        ["text regex"] = comment.Text
            //    });
            //    throw;
            //}
        }

        private Task UpdateItemDiscussionAsync(long? itemDiscussionId, IEnumerable<long> userIds, Box box)
        {
            if (!itemDiscussionId.HasValue)
            {
                return Task.CompletedTask;
            }
            var itemDiscussion = _itemCommentRepository.Load(itemDiscussionId.Value);
            DoUpdateLoop(userIds, u => Updates.UpdateItemDiscussion(u, box, itemDiscussion));
            return Task.CompletedTask;
        }

        private Task UpdateItemReplyDiscussionAsync(long? itemReplyDiscussionId, IEnumerable<long> userIds, Box box)
        {
            if (!itemReplyDiscussionId.HasValue)
            {
                return Task.CompletedTask;
            }
            var itemReplyDiscussion = _itemCommentReplyRepository.Load(itemReplyDiscussionId.Value);
            DoUpdateLoop(userIds, u => Updates.UpdateItemDiscussionReply(u, box, itemReplyDiscussion));
            var userDiscussion = itemReplyDiscussion.Parent.Author;
            if (userDiscussion.Id == itemReplyDiscussion.Author.Id || userDiscussion.EmailSendSettings != Infrastructure.Enums.EmailSend.CanSend)
            {
                return Task.CompletedTask;
            }
            return _queueProvider.InsertMessageToMailNewAsync(new ReplyToCommentData(userDiscussion.Email,
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
            var quizDiscussion = _quizDiscussionRepository.Load(quizDiscussionId.Value);
            DoUpdateLoop(userIds, u => Updates.UpdateQuizDiscussion(u, box, quizDiscussion));
            return Task.CompletedTask;
        }

        //private static string BuildBoxTag(long id)
        //{
        //    return $"_BoxId:{id}";
        //}
    }
}
