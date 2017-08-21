using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class DepartmentSettings
    {
        [Required]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public LibraryNodeSetting? Settings { get; set; }

        public override string ToString()
        {
            return string.Format("id: {0} name: {1} settings {2}", Id, Name, Settings);
        }
    }
}