﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class DocumentSearchDto
    {
        [EntityBind(nameof(Document.Id))]
        public long ItemId { get; set; }
        [EntityBind(nameof(Document.Name))]
        public string Name { get; set; }

        [EntityBind(nameof(Document.OldCourse.Id))]
        public string Course { get; set; }
        [EntityBind(nameof(Document.User.Country))]
        public string? Country { get; set; }


        [EntityBind(nameof(Document.User.SbCountry))]
        public Country SbCountry { get; set; }
       
        [EntityBind(nameof(Document.TimeStamp.CreationTime))]
        public DateTime? DateTime { get; set; }
        [EntityBind(nameof(Document.Status.State))]
        public ItemState State { get; set; }
        [EntityBind(nameof(Document.DocumentType))]
        public DocumentType Type { get; set; }


    }
}