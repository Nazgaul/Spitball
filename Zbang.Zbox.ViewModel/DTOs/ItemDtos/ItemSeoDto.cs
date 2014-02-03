using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class ItemSeoDto
    {
        public long Id { get; set; }

        public long BoxId { get; set; }
        public string Name { get; set; }

        public string BoxName { get; set; }

        public string UniversityName { get; set; }
    }
}
