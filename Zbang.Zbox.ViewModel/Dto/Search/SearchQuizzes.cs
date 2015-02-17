using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Dto.Search
{
    public class SearchQuizzes
    {
        public SearchQuizzes()
        {

        }
        public SearchQuizzes(string name, long id, string content,
            string boxName, string uniName, string url, string nameWithoutHighLight)
        {
            Name = name;
            Id = id;
            Content = content;
            UniName = uniName;
            Boxname = boxName;
            Url = url;
            NameWithoutHighLight = nameWithoutHighLight;
        }
        public string Name { get; set; }

        public string NameWithoutHighLight { get; set; }
        public long Id { get; set; }
        public string Content { get; set; }

        public string Boxname { get; set; }
        public string UniName { get; set; }
        public string Url { get; set; }
    }
}
