using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Document.PurchaseDocument
{
    public class PurchaseDocumentCommandHandler : ICommandHandler<PurchaseDocumentCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IRepository<Core.Entities.Document> _documentRepository;

        public PurchaseDocumentCommandHandler(IRegularUserRepository userRepository, ITransactionRepository transactionRepository, IRepository<Core.Entities.Document> documentRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(PurchaseDocumentCommand message, CancellationToken token)
        {
            var document = await _documentRepository.LoadAsync(message.DocumentId, token);
            if (document.Price == 0)
            {
                throw new ArgumentException();
            }

            var balance = await _transactionRepository.GetBalanceAsync(message.UserId,token);
            if (balance < document.Price)
            {
                throw new InsufficientFundException();
            }

            var purchaseUser = await _userRepository.LoadAsync(message.UserId, token);
            var t = new Core.Entities.Transaction(TransactionActionType.PurchaseDocument, TransactionType.Spent,
                -document.Price, purchaseUser)
            {
                Document = document
            };
            await _transactionRepository.AddAsync(t, token);
            if (document.User.Actual is RegularUser p)
            {
                var t2 = new Core.Entities.Transaction(TransactionActionType.SoldDocument, TransactionType.Earned,
                    document.Price,p)
                {
                    Document = document
                };
                await _transactionRepository.AddAsync(t2, token);
            }
        }
    }
}