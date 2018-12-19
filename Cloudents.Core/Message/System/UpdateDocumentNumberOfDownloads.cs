﻿namespace Cloudents.Application.Message.System
{
    public class UpdateDocumentNumberOfDownloads : ISystemQueueMessage
    {
        public UpdateDocumentNumberOfDownloads(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}