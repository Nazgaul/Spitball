//using Cloudents.Core.Entities;
//using Cloudents.Core.Interfaces;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Command.Documents.ChangePrice
//{
//    public class ChangeDocumentPriceCommandHandler : ICommandHandler<ChangeDocumentPriceCommand>
//    {
//        private readonly IRepository<Document> _documentRepository;

//        public ChangeDocumentPriceCommandHandler(IRepository<Document> documentRepository)
//        {
//            _documentRepository = documentRepository;
//        }

//        public async Task ExecuteAsync(ChangeDocumentPriceCommand message, CancellationToken token)
//        {
//            var document = await _documentRepository.LoadAsync(message.DocumentId, token);

//            if (document.User.Id != message.UserId)
//            {
//                throw new ArgumentException();
//            }
//            document.ChangePrice(message.Price);
//            await _documentRepository.UpdateAsync(document, token);

//        }
//    }
//}
