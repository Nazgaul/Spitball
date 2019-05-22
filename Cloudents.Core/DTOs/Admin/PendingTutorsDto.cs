using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
        public string Courses { get; set; }
    }
}
