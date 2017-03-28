using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class PublishFlashcardCommandHandler : ICommandHandlerAsync<PublishFlashcardCommand>
    {
        private readonly IDocumentDbRepository<Flashcard> m_FlashcardRepository;
        private readonly IRepository<FlashcardMeta> m_FlashcardMetaRepository;
        private readonly IItemRepository m_ItemRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Comment> m_CommentRepository;
        private readonly IGuidIdGenerator m_IdGenerator;
        private readonly IQueueProvider m_QueueProvider;


        public PublishFlashcardCommandHandler(IDocumentDbRepository<Flashcard> flashcardRepository, IRepository<FlashcardMeta> flashcardMetaRepository, IUserRepository userRepository, IBoxRepository boxRepository, IItemRepository itemRepository, IRepository<Comment> commentRepository, IGuidIdGenerator idGenerator, IQueueProvider queueProvider)
        {
            m_FlashcardRepository = flashcardRepository;
            m_FlashcardMetaRepository = flashcardMetaRepository;
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_ItemRepository = itemRepository;
            m_CommentRepository = commentRepository;
            m_IdGenerator = idGenerator;
            m_QueueProvider = queueProvider;
        }

        public async Task HandleAsync(PublishFlashcardCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var box = m_BoxRepository.Load(message.Flashcard.BoxId);
            var flashCardWithSameName = box.Flashcards.FirstOrDefault(w => w.Name == message.Flashcard.Name && w.Publish && w.Id != message.Flashcard.Id);
            if (flashCardWithSameName != null)
            {
                throw new ArgumentException("same flashcard name");
            }
            var user = m_UserRepository.Load(message.Flashcard.UserId);
            var flashcard = m_FlashcardMetaRepository.Load(message.Flashcard.Id);
            if (flashcard.User != user)
            {
                throw new UnauthorizedAccessException();
            }
            if (flashcard.Box != box)
            {
                throw new UnauthorizedAccessException();
            }




            flashcard.Name = message.Flashcard.Name;
            if (message.Flashcard.Cards != null)
            {
                flashcard.CardCount = message.Flashcard.Cards.Count();
            }
            flashcard.DateTimeUser.UpdateUserTime(message.Flashcard.Id);
            flashcard.Pins?.Clear();
            flashcard.Publish = true;
            m_FlashcardMetaRepository.Save(flashcard);


            if (flashcard.Comment == null) //if edit flashcard already have comment
            {
                var comment = m_ItemRepository.GetPreviousCommentId(box.Id, user.Id) ??
                              new Comment(user, null, box, m_IdGenerator.GetId(), null, FeedType.AddedItems, false);
                comment.AddFlashcard(flashcard);
                m_CommentRepository.Save(comment);
            }


            box.UpdateFlashcardCount();
            m_BoxRepository.Save(box);
            var t5 = m_QueueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id));

            var t1 = m_FlashcardRepository.UpdateItemAsync(message.Flashcard.id, message.Flashcard);
            await Task.WhenAll(t1, t5);
        }
    }
}