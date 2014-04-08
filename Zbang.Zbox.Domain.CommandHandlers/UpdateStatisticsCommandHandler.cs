using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateStatisticsCommandHandler : ICommandHandler<UpdateStatisticsCommand>
    {
        private IRepository<Item> m_ItemRepository;
        private IRepository<Zbang.Zbox.Domain.Quiz> m_QuizRepository;
        private IUserRepository m_UserRepository;
        public UpdateStatisticsCommandHandler(IRepository<Item> itemRepository, IUserRepository userRepository,
            IRepository<Zbang.Zbox.Domain.Quiz> quizRepository)
        {
            m_ItemRepository = itemRepository;
            m_UserRepository = userRepository;
            m_QuizRepository = quizRepository;
        }
        public void Handle(UpdateStatisticsCommand message)
        {
            Throw.OnNull(message, "message");

            if (message.UserId > 0)
            {
                var user = m_UserRepository.Load(message.UserId);
                user.LastAccessTime = message.StatTime;
                m_UserRepository.Save(user);
            }



            foreach (var itemId in message.ItemId)
            {
                if (itemId.Action == Infrastructure.Enums.StatisticsAction.Quiz)
                {
                    var quiz = m_QuizRepository.Load(itemId.ItemId);
                    quiz.UpdateNumberOfViews();
                    m_QuizRepository.Save(quiz);
                    break;
                }
                var item = m_ItemRepository.Get(itemId.ItemId);// we use get because we need to cast to File and get proxy
                if (item == null)
                {
                    TraceLog.WriteInfo("itemId is null " + itemId);
                    continue;
                }
                if (itemId.ItemId == 0)
                {
                    TraceLog.WriteInfo("itemId is 0 " + itemId);
                    continue;
                }
                

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
