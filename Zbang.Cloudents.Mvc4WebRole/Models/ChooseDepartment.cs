﻿
using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class ChooseDepartment
    {
        [Required(ErrorMessageResourceType = typeof(CreateUniversityResources), ErrorMessageResourceName = "FieldRequired")]
        public string Name { get; set; }
    }
}