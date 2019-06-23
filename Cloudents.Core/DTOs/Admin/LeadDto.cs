using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class LeadDto
    {
        [EntityBind(nameof(Lead.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(Lead.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(Lead.Phone))]
        public string Phone { get; set; }
        [EntityBind(nameof(Lead.Text))]
        public string Text { get; set; }
        [EntityBind(nameof(Lead.Course.Id))]
        public string Course { get; set; }
        [EntityBind(nameof(Lead.University.Name))]
        public string University { get; set; }
        [EntityBind(nameof(Lead.Referer))]
        public string Referer { get; set; }
    }
}
