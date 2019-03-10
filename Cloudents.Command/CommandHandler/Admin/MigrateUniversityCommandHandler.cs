using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class MigrateUniversityCommandHandler : ICommandHandler<MigrateUniversityCommand>
    {
        private readonly IUniversityRepository _universityRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IRepository<User> _userRepository;
        public MigrateUniversityCommandHandler(IUniversityRepository universityRepository, IDocumentRepository documentRepository,
                IQuestionRepository questionRepository, IRepository<User> userRepository)
        {
            _universityRepository = universityRepository;
            _documentRepository = documentRepository;
            _questionRepository = questionRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(MigrateUniversityCommand message, CancellationToken token)
        {
            var UniversityToRemove = await _universityRepository.LoadAsync(message.UniversityToRemove, token);
            var UniversityToKeep = await _universityRepository.LoadAsync(message.UniversityToKeep, token);

            foreach (var doc in UniversityToRemove.Documents)
            {
                doc.University = UniversityToKeep;
                await _documentRepository.UpdateAsync(doc, token);
            }

            foreach (var question in UniversityToRemove.Questions)
            {
                question.University = UniversityToKeep;
                await _questionRepository.UpdateAsync(question, token);
            }

            foreach (var user in UniversityToRemove.Users)
            {
                user.University = UniversityToKeep;
               
                await _userRepository.UpdateAsync(user, token);
            }

            await _universityRepository.DeleteAsync(UniversityToRemove, token);
        }
    }
}
