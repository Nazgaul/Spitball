using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Documents.ChangePrice
{
    public class ChangePriceCommandHandler : ICommandHandler<ChangePriceCommand>
    {
        private readonly IRepository<Document> _documentRepository;

        public ChangePriceCommandHandler(IRepository<Document> documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(ChangePriceCommand message, CancellationToken token)
        {
            var document = await _documentRepository.LoadAsync(message.DocumentId, token);

            if (document.User.Id != message.UserId)
            {
                throw new ArgumentException();
            }

            document.Price = message.Price;
            await _documentRepository.UpdateAsync(document, token);

        }
    }
}
