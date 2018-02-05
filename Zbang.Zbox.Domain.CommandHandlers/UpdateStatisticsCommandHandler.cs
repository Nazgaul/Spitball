
using System;
using System.Collections.Generic;
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
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<Quiz> m_QuizRepository;
        private readonly IRepository<FlashcardMeta> _flashcardRepository;
        private readonly IQueueProvider _queueProvider;
        private readonly ILogger m_Logger;

        public UpdateStatisticsCommandHandler(IRepository<Item> itemRepository,
            IRepository<Quiz> quizRepository, IRepository<FlashcardMeta> flashcardRepository, IQueueProvider queueProvider, ILogger logger)
        {
            _itemRepository = itemRepository;
            m_QuizRepository = quizRepository;
            _flashcardRepository = flashcardRepository;
            _queueProvider = queueProvider;
            m_Logger = logger;
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
                    var flashcard = _flashcardRepository.Load(itemId.ItemId);
                    if (flashcard.User.Id == message.UserId)
                    {
                        return t;
                    }
                    flashcard.UpdateNumberOfViews();
                    _flashcardRepository.Save(flashcard);
                    return UpdateReputationAsync(flashcard.User.Id);
                }
                catch (ApplicationException ex)
                {
                    m_Logger.Exception(ex, new Dictionary<string, string>
                    {
                        ["id"] = itemId.ItemId.ToString(),
                    });
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
                    m_Logger.Exception(ex, new Dictionary<string, string>
                    {
                        ["id"] = itemId.ItemId.ToString(),
                    });
                }
                return t;
            }
            var item = _itemRepository.Get(itemId.ItemId); // we use get because we need to cast to File and get proxy
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
                if (!(item is File file))
                {
                    item.IncreaseNumberOfViews();
                }
                else
                {
                    file.IncreaseNumberOfDownloads();
                }
            }
            _itemRepository.Save(item);
            return UpdateReputationAsync(item.UploaderId);
        }

        private Task UpdateReputationAsync(long userId)
        {
            return _queueProvider.InsertMessageToTransactionAsync(new ReputationData(userId));
        }
    }
}
