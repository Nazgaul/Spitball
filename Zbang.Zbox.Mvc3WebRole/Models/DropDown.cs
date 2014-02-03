using System.Collections.Generic;
using System.Web.Mvc;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class DropDown
    {
        public string SelectedCategoryId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}