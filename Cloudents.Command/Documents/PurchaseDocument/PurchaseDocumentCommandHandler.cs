﻿using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Documents.PurchaseDocument
{
    public class PurchaseDocumentCommandHandler : ICommandHandler<PurchaseDocumentCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IRepository<Document> _documentRepository;

        public PurchaseDocumentCommandHandler(IRepository<User> userRepository, ITransactionRepository transactionRepository, IRepository<Document> documentRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _documentRepository = documentRepository;
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

            purchaseUser.MakeTransaction(TransactionType2.Spend(document.Price,TransactionActionType.PurchaseDocument),document: document);
            document.User.MakeTransaction(TransactionType2.Earn(document.Price,TransactionActionType.SoldDocument),document:document);

            await _userRepository.UpdateAsync(purchaseUser, token);
            await _userRepository.UpdateAsync(document.User, token);

        }
    }
}