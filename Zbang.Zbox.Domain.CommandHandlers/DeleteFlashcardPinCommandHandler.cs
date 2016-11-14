using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteFlashcardPinCommandHandler : ICommandHandler<DeleteFlashcardPinCommand>
    {
        private readonly IFlashcardPinRepository m_FlashcardPinRepository;

        public DeleteFlashcardPinCommandHandler(IFlashcardPinRepository flashcardPinRepository)
        {
            m_FlashcardPinRepository = flashcardPinRepository;
        }

        public void Handle(DeleteFlashcardPinCommand message)
        {

            var pin = m_FlashcardPinRepository.GetUserPin(message.UserId, message.FlashCardId, message.Index);
            m_FlashcardPinRepository.Delete(pin);
        }
    }
}
