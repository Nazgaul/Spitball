using System.Collections.Generic;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class DropDown
    {
        public string SelectedCategoryId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}