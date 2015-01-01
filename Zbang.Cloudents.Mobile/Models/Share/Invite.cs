﻿using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mobile.Models.Resources;

namespace Zbang.Cloudents.Mobile.Models.Share
{
    public class Invite
    {
        [Required(ErrorMessageResourceType = typeof(InvitationResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(InvitationResources), Name = "To")]
        public string[] Recepients { get; set; }

        [Required]
        public long BoxId { get; set; }
    }
}