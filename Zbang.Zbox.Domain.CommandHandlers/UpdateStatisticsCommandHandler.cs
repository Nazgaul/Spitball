
using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateStatisticsCommandHandler : ICommandHandlerAsync<UpdateStatisticsCommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<FlashcardMeta> m_FlashcardRepository;
        private readonly IQueueProvider m_QueueProvider;
        public UpdateStatisticsCommandHandler(IRepository<Item> itemRepository,
            IRepository<Domain.Quiz> quizRepository, IRepository<FlashcardMeta> flashcardRepository, IQueueProvider queueProvider)
        {
            m_ItemRepository = itemRepository;
            m_QuizRepository = quizRepository;
            m_FlashcardRepository = flashcardRepository;
            m_QueueProvider = queueProvider;
        }

        public Task HandleAsync(UpdateStatisticsCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var t = Task.CompletedTask;
            if (message.ItemId == null) return t;
            var itemId = message.ItemId;
            if (itemId.ItemId == 0)
            {
                return t;
            }

            if (itemId.Action == Infrastructure.Enums.StatisticsAction.Flashcard)
            {
                try
                {
                    var flashcard = m_FlashcardRepository.Load(itemId.ItemId);
                    if (flashcard.User.Id == message.UserId)
                    {
                        return t;
                    }
                    flashcard.UpdateNumberOfViews();
                    m_FlashcardRepository.Save(flashcard);
                    return UpdateReputationAsync(flashcard.User.Id);
                }
                catch (ApplicationException ex)
                {
                    TraceLog.WriteError("On update quiz views itemId:" + itemId.ItemId, ex);
                }
            }
            if (itemId.Action == Infrastructure.Enums.StatisticsAction.Quiz)
            {
                try
                {
                    var quiz = m_QuizRepository.Load(itemId.ItemId);
                    if (quiz.User.Id == message.UserId)
                    {
                        return t;
                    }
                    quiz.UpdateNumberOfViews();
                    m_QuizRepository.Save(quiz);
                    return UpdateReputationAsync(quiz.User.Id);
                }
                catch (ApplicationException ex)
                {
                    TraceLog.WriteError("On update quiz views itemId:" + itemId.ItemId, ex);
                }
                return t;
            }
            var item = m_ItemRepository.Get(itemId.ItemId); // we use get because we need to cast to File and get proxy
            if (item == null)
            {
                return t;
            }

            if (item.User.Id == message.UserId)
            {
                return t;
            }
            item.ShouldMakeDirty = () => true;
            if (itemId.Action == Infrastructure.Enums.StatisticsAction.View)
            {
                item.IncreaseNumberOfViews();
            }
            else
            {
                var file = item as File;
                if (file == null)
                {
                    item.IncreaseNumberOfViews();
                }
                else
                {
                    file.IncreaseNumberOfDownloads();
                }
            }
            m_ItemRepository.Save(item);
            return UpdateReputationAsync(item.UploaderId);
        }

        private Task UpdateReputationAsync(long userId)
        {
            return m_QueueProvider.InsertMessageToTransactionAsync(new ReputationData(userId));
        }
    }
}
