using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Search
{
    public class SearchBoxes
    {
        private string m_Image;

        public string Image
        {
            get { return m_Image; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    m_Image = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(value);
                }
            }
        }
        public string Name { get; set; }
        public string Proffessor { get; set; }
        public string CourseCode { get; set; }
        public long Id { get; set; }
        public string Universityname { get; set; }

        public string Url { get; set; }
    }
}
