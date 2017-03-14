using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc2Jared.Models
{
    public class SearchItemWatson : IPagedQuery
    {
        public SearchItemWatson(int pageNumber = 0, int rowsPerPage = 50)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
        }
        public string Name { get; set; }

        public string University { get; set; }
        public string Department { get; set; }
        public string Box { get; set; }
        public string BoxId { get; set; }
        public bool isReviewed { get; set; }
        public bool isSearchType { get; set; }
        public int RowsPerPage { get;  }
        public int PageNumber { get; }
    }
}