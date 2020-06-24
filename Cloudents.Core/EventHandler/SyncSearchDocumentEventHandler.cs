//using System.Collections.Generic;
//using Cloudents.Core.DTOs.SearchSync;
//using Cloudents.Core.Event;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Message.System;
//using Cloudents.Core.Storage;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Core.EventHandler
//{
//    public class SyncSearchDocumentEventHandler : IEventHandler<DocumentCreatedEvent>,
//        IEventHandler<DocumentDeletedEvent>,
//        //IEventHandler<DocumentUndeletedEvent>,
//        IEventHandler<DeleteUserEvent>
//    {
//        private readonly IQueueProvider _queueProvider;

//        public SyncSearchDocumentEventHandler(IQueueProvider queueProvider)
//        {
//            _queueProvider = queueProvider;
//        }

//        public Task HandleAsync(DocumentCreatedEvent eventMessage, CancellationToken token)
//        {
//            var doc = new DocumentSearchDto
//            {
//                Country = eventMessage.Document.User.Country?.ToUpperInvariant(),

//                Course = eventMessage.Document.Course.Id.ToUpperInvariant(),
//                DateTime = eventMessage.Document.TimeStamp.UpdateTime,
//                ItemId = eventMessage.Document.Id,
//                Name = eventMessage.Document.Name,
//                Type = eventMessage.Document.DocumentType,
//                SbCountry = eventMessage.Document.User.SbCountry
//            };
//            return _queueProvider.InsertMessageAsync(new DocumentSearchMessage(doc, true), token);
//        }



//        //public Task HandleAsync(DocumentUndeletedEvent eventMessage, CancellationToken token)
//        //{
//        //    var doc = new DocumentSearchDto
//        //    {
//        //        Country = eventMessage.Document.User.Country?.ToUpperInvariant(),
//        //        Course = eventMessage.Document.Course.Id.ToUpperInvariant(),
//        //        DateTime = eventMessage.Document.TimeStamp.UpdateTime,
//        //        ItemId = eventMessage.Document.Id,
//        //        Name = eventMessage.Document.Name,
//        //        Type = eventMessage.Document.DocumentType,
//        //        SbCountry = eventMessage.Document.User.SbCountry
//        //    };
//        //    return _queueProvider.InsertMessageAsync(new DocumentSearchMessage(doc, true), token);
//        //}

//        public Task HandleAsync(DocumentDeletedEvent eventMessage, CancellationToken token)
//        {
//            return DeleteDocumentMessageAsync(eventMessage.Document.Id, token);
//        }

//        private Task DeleteDocumentMessageAsync(long id, CancellationToken token)
//        {
//            var doc = new DocumentSearchDto
//            {
//                ItemId = id
//            };
//            return _queueProvider.InsertMessageAsync(new DocumentSearchMessage(doc, false), token);
//        }

//        public Task HandleAsync(DeleteUserEvent eventMessage, CancellationToken token)
//        {
//            var list = new List<Task>();
//            foreach (var document in eventMessage.User.Documents)
//            {
//                list.Add(DeleteDocumentMessageAsync(document.Id, token));
//            }

//            return Task.WhenAll(list);
//        }
//    }
//}