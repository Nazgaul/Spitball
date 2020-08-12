﻿using System;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.Documents
{
    public class DocumentFeedDto 
    {
        [EntityBind(nameof(Document.Id))]
        public long Id { get; set; }
        public string Title { get; set; }

        public string Preview { get; set; }

        public DocumentType DocumentType { get; set; }

    }

    public class CourseEditDocumentDto
    {
        public long Id { get; set; }
        public string Title { get; set; }

        public bool Visible { get; set; }


    }

    public class CourseEditCouponDto
    {
        public Guid Id { get; set; }
        public double Value { get; set; }
        public string Code { get; set; }
        public CouponType CouponType { get; set; }
        public DateTime? Expiration { get; set; }
    }

    
}