using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class SaveUserFlashcardCommandHandler : ICommandHandler<SaveUserFlashcardCommand>
    {
        private readonly IRepository<FlashcardSolve> m_FlashcardSolveRepository;
        private readonly IRepository<FlashcardMeta> m_FlashcardRepository;
        private readonly IRepository<User> m_UserRepository;
        private readonly IGuidIdGenerator m_IdGenerator;

        public SaveUserFlashcardCommandHandler(IRepository<FlashcardSolve> flashcardSolveRepository, IRepository<FlashcardMeta> flashcardRepository, IRepository<User> userRepository, IGuidIdGenerator idGenerator)
        {
            m_FlashcardSolveRepository = flashcardSolveRepository;
            m_FlashcardRepository = flashcardRepository;
            m_UserRepository = userRepository;
            m_IdGenerator = idGenerator;
        }

        public void Handle(SaveUserFlashcardCommand message)
        {
            var solve =
                m_FlashcardSolveRepository.GetQueryable()
                    .FirstOrDefault(f => f.User.Id == message.UserId && f.Flashcard.Id == message.FlashcardId);
            if (solve == null)
            {
                var user = m_UserRepository.Load(message.UserId);
                var flashcard = m_FlashcardRepository.Load(message.FlashcardId);
                solve = new FlashcardSolve(m_IdGenerator.GetId(), user, flashcard);

            }
            solve.DateTime = DateTime.UtcNow;
            m_FlashcardSolveRepository.Save(solve);
        }
    }
}
