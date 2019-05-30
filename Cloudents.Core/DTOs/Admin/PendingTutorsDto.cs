﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class PendingTutorsDto
    {
        [EntityBind(nameof(Tutor.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(RegularUser.FirstName))]
        public string FirstName { get; set; }
        [EntityBind(nameof(RegularUser.LastName))]
        public string LastName { get; set; }
        [EntityBind(nameof(Tutor.Bio))]
        public string Bio { get; set; }
        [EntityBind(nameof(Tutor.Price))]
        public decimal Price { get; set; }
        [EntityBind(nameof(User.Email))]
        public string Email{ get; set; }

        [EntityBind(nameof(User.Image))]
        public string Image { get; set; }
        public string Courses { get; set; }
        [EntityBind(nameof(Tutor.Created))]
        public DateTime? Created { get; set; }
    }
}