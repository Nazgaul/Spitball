using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Documents.PurchaseDocument
{
    public class PurchaseDocumentCommandHandler : ICommandHandler<PurchaseDocumentCommand>
    {
        private readonly IRepository<BaseUser> _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IRepository<Document> _documentRepository;
        private readonly ICacheProvider _cacheProvider;

        public PurchaseDocumentCommandHandler(IRepository<BaseUser> userRepository, ITransactionRepository transactionRepository, 
            IRepository<Document> documentRepository, ICacheProvider cacheProvider)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _documentRepository = documentRepository;
            _cacheProvider = cacheProvider;
        }

        public async Task ExecuteAsync(PurchaseDocumentCommand message, CancellationToken token)
        {
            var document = await _documentRepository.LoadAsync(message.DocumentId, token);
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

            DocumentTransaction.MakerTransaction(purchaseUser, document.User, document);

            document.User.AddFollower(purchaseUser);
            _cacheProvider.DeleteRegion("document-by-id");
             await _userRepository.UpdateAsync(purchaseUser, token);
            await _userRepository.UpdateAsync(document.User, token);

        }
    }
}