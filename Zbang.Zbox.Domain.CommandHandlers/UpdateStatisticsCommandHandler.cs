
using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateStatisticsCommandHandler : ICommandHandler<UpdateStatisticsCommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<FlashCardMeta> m_FlashcardRepository;
        public UpdateStatisticsCommandHandler(IRepository<Item> itemRepository, 
            IRepository<Domain.Quiz> quizRepository, IRepository<FlashCardMeta> flashcardRepository)
        {
            m_ItemRepository = itemRepository;
            m_QuizRepository = quizRepository;
            m_FlashcardRepository = flashcardRepository;
        }
        public void Handle(UpdateStatisticsCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            
            //if (message.UserId > 0)
            //{
            //    var user = m_UserRepository.Load(message.UserId);
            //    user.LastAccessTime = message.StatTime;
            //    m_UserRepository.Save(user);
            //}


            if (message.ItemId == null) return;
            foreach (var itemId in message.ItemId)
            {
                if (itemId.ItemId == 0)
                {
                    TraceLog.WriteInfo("itemId is 0 " + itemId);
                    continue;
                }
                switch (itemId.Action)
                {
                    case Infrastructure.Enums.StatisticsAction.Flashcard:
                        try
                        {
                            var flashcard = m_FlashcardRepository.Load(itemId.ItemId);
                            flashcard.UpdateNumberOfViews();
                            flashcard.ShouldMakeDirty = () => false;
                            m_FlashcardRepository.Save(flashcard);
                        }
                        catch (ApplicationException ex)
                        {
                            TraceLog.WriteError("On update quiz views itemid:" + itemId.ItemId, ex);
                        }
                        continue;
                    case Infrastructure.Enums.StatisticsAction.Quiz:
                        try
                        {
                            var quiz = m_QuizRepository.Load(itemId.ItemId);
                            quiz.UpdateNumberOfViews();
                            quiz.ShouldMakeDirty = () => false;
                            m_QuizRepository.Save(quiz);
                        }
                        catch (ApplicationException ex)
                        {
                            TraceLog.WriteError("On update quiz views itemid:" + itemId.ItemId, ex);
                        }
                        continue;
                }
                var item = m_ItemRepository.Get(itemId.ItemId);// we use get because we need to cast to File and get proxy
                if (item == null)
                {
                    TraceLog.WriteInfo("itemId is null " + itemId);
                    continue;
                }

                item.ShouldMakeDirty = () => false;

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

            }
        }
    }
}
