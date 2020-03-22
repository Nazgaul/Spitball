using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Documents.PurchaseDocument
{
    public class PurchaseDocumentCommandHandler : ICommandHandler<PurchaseDocumentCommand>
    {
        private readonly IRepository<BaseUser> _baseUserRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IRepository<Document> _documentRepository;

        public PurchaseDocumentCommandHandler(IRepository<BaseUser> userRepository, ITransactionRepository transactionRepository,
            IRepository<Document> documentRepository, IRepository<User> userRepository1)
        {
            _baseUserRepository = userRepository;
            _transactionRepository = transactionRepository;
            _documentRepository = documentRepository;
            _userRepository = userRepository1;
        }

        public async Task ExecuteAsync(PurchaseDocumentCommand message, CancellationToken token)
        {
            var document = await _documentRepository.GetAsync(message.DocumentId, token);
            if (document is null)
            {
                throw new NotFoundException();
            }
            if (document.Price == 0 || document.User.Id == message.UserId)
            {
                throw new ArgumentException();
            }

            var balance = await _transactionRepository.GetBalanceAsync(message.UserId, token);
            if (balance < document.Price)
            {
                throw new InsufficientFundException();
            }


            var purchaseUser = await _userRepository.LoadAsync(message.UserId, token);

            document.PurchaseDocument(purchaseUser);
            //DocumentTransaction.MakerTransaction(purchaseUser, document.User, document);


            document.User.AddFollower(purchaseUser);
            await _userRepository.UpdateAsync(purchaseUser, token);
            await _baseUserRepository.UpdateAsync(document.User, token);

        }
    }
}