﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs.SearchSync;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Message.System;
using Cloudents.Application.Storage;

namespace Cloudents.Application.EventHandler
{
    public class SyncSearchDocumentEventHandler : IEventHandler<DocumentCreatedEvent>,
        IEventHandler<DocumentDeletedEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public SyncSearchDocumentEventHandler(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(DocumentCreatedEvent eventMessage, CancellationToken token)
        {
            var doc = new DocumentSearchDto
            {
                UniversityId = eventMessage.Document.University.Id,
                UniversityName = eventMessage.Document.University.Name,
                Country = eventMessage.Document.University.Country.ToUpperInvariant(),
                Course = eventMessage.Document.Course.Name.ToUpperInvariant(),
                DateTime = eventMessage.Document.TimeStamp.UpdateTime,
                ItemId =  eventMessage.Document.Id,
                Name = eventMessage.Document.Name,
                TagsArray = eventMessage.Document.Tags.Select(s => s.Name.ToUpperInvariant()).ToArray(),
                Type = eventMessage.Document.Type
            };
            return _queueProvider.InsertMessageAsync(new DocumentSearchMessage(doc, true), token);
        }

        public Task HandleAsync(DocumentDeletedEvent eventMessage, CancellationToken token)
        {
            var doc = new DocumentSearchDto
            {
                ItemId = eventMessage.Document.Id
            };
            return _queueProvider.InsertMessageAsync(new DocumentSearchMessage(doc, false), token);
        }
    }
}