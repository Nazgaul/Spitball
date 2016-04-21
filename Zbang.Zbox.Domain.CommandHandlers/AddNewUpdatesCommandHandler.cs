﻿using System;
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
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IRepository<ItemCommentReply> m_ItemCommentReplyRepository;
        private readonly IRepository<QuizDiscussion> m_QuizDiscussionRepository;
        private readonly ISendPush m_SendPush;

        public AddNewUpdatesCommandHandler(
            IBoxRepository boxRepository,
            IRepository<Item> itemRepository,
            IRepository<CommentReplies> replyRepository,
            IRepository<Comment> commentRepository,
            IRepository<Updates> updatesRepository,
            IRepository<Domain.Quiz> quizRepository, ISendPush sendPush, IUserRepository userRepository, IRepository<ItemComment> itemCommentRepository, IRepository<ItemCommentReply> itemCommentReplyRepository, IRepository<QuizDiscussion> quizDiscussionRepository)
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
        }
        public Task HandleAsync(AddNewUpdatesCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var box = m_BoxRepository.Load(message.BoxId);
            var usersToUpdate = box.UserBoxRelationship.Where(w => w.User.Id != message.UserId).Select(s => s.UserId).ToList();
            if (usersToUpdate.Count == 0)
            {
                return Infrastructure.Extensions.TaskExtensions.CompletedTask;
            }
            
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
                //var user = ;
                //var user = new User(userId);
                var newUpdate = update(m_UserRepository.Load(userId));
                m_UpdatesRepository.Save(newUpdate);
            }
        }

        private Task UpdateQuizAsync(long? quizId, IEnumerable<long> userIds, Box box)
        {
            if (!quizId.HasValue)
            {
                return Infrastructure.Extensions.TaskExtensions.CompletedTask;
            }
            var quiz = m_QuizRepository.Load(quizId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, quiz));
            return Infrastructure.Extensions.TaskExtensions.CompletedTask;
        }

        private Task UpdateItemAsync(long? itemId, IList<long> userIds, Box box)
        {
            if (!itemId.HasValue)
            {
                return Infrastructure.Extensions.TaskExtensions.CompletedTask;
            }
            var item = m_ItemRepository.Load(itemId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, item));
            return m_SendPush.SendAddItemNotification(item.Uploader.Name, box.Name, box.Id, userIds);
        }



        private Task UpdateReplyAsync(Guid? replyId, IList<long> userIds, Box box)
        {
            if (!replyId.HasValue)
            {
                return Infrastructure.Extensions.TaskExtensions.CompletedTask;
            }
            var reply = m_ReplyRepository.Load(replyId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, reply));
            return m_SendPush.SendAddReplyNotification(reply.User.Name, reply.Text, box.Name, box.Id, reply.Question.Id, userIds);
        }

        private Task UpdateCommentAsync(Guid? commentId, IList<long> userIds, Box box)
        {
            if (!commentId.HasValue)
            {
                return Infrastructure.Extensions.TaskExtensions.CompletedTask;
            }
            var comment = m_CommentRepository.Load(commentId.Value);
            DoUpdateLoop(userIds, u => new Updates(u, box, comment));
            if (string.IsNullOrEmpty(comment.Text))
            {
                return Infrastructure.Extensions.TaskExtensions.CompletedTask;
            }
            try
            {
                var removeHtmlRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
                var textToPush = removeHtmlRegex.Replace(comment.Text, string.Empty);
                if (string.IsNullOrEmpty(textToPush))
                {
                    return Infrastructure.Extensions.TaskExtensions.CompletedTask;
                }

                return m_SendPush.SendAddPostNotification(comment.User.Name, textToPush, box.Name, box.Id, userIds);
            }
            catch (ArgumentNullException ex)
            {
                TraceLog.WriteError("regex error text: " + comment.Text, ex);
                throw;
            }
        }

        private Task UpdateItemDiscussionAsync(long? itemDiscussionId, IList<long> userIds, Box box)
        {
            if (!itemDiscussionId.HasValue)
            {
                return Infrastructure.Extensions.TaskExtensions.CompletedTask;
            }
            var itemDiscussion = m_ItemCommentRepository.Load(itemDiscussionId.Value);
            DoUpdateLoop(userIds, u => Updates.UpdateItemDiscussion(u,box,itemDiscussion) );
            return Infrastructure.Extensions.TaskExtensions.CompletedTask;
        }
        private Task UpdateItemReplyDiscussionAsync(long? itemReplyDiscussionId, IList<long> userIds, Box box)
        {
            if (!itemReplyDiscussionId.HasValue)
            {
                return Infrastructure.Extensions.TaskExtensions.CompletedTask;
            }
            var itemReplyDiscussion = m_ItemCommentReplyRepository.Load(itemReplyDiscussionId.Value);
            DoUpdateLoop(userIds, u => Updates.UpdateItemDiscussionReply(u, box, itemReplyDiscussion));
            return Infrastructure.Extensions.TaskExtensions.CompletedTask;
        }
        private Task UpdateQuizDiscussionAsync(Guid? quizDiscussionId, IList<long> userIds, Box box)
        {
            if (!quizDiscussionId.HasValue)
            {
                return Infrastructure.Extensions.TaskExtensions.CompletedTask;
            }
            var quizDiscussion = m_QuizDiscussionRepository.Load(quizDiscussionId.Value);
            DoUpdateLoop(userIds, u => Updates.UpdateQuizDiscussion(u, box, quizDiscussion));
            return Infrastructure.Extensions.TaskExtensions.CompletedTask;
        }

    }
}
