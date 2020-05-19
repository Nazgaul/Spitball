﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    public class SetSessionDurationRequest
    {
        [Required, FromRoute]
        public Guid SessionId { get; set; }

        public long UserId { get; set; }

        [Required]
        [Range(1, 1000)]
        public long DurationInMinutes { get; set; }

        [Required]
        public double Price { get; set; }

    }


    public class OfflinePaymentRequest
    {
        public long UserId { get; set; }
        [Required]
        [Range(1, 1000)]
        public long DurationInMinutes { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
