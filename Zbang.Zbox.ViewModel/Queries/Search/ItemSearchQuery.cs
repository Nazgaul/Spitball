using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class ItemSearchQuery : IPagedQuery, IUserQuery
    {
        public ItemSearchQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50)
        {
            UniversityId = universityId;
            Term = term;
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            UserId = userId;
        }

        public long UserId { get; private set; }

        public long UniversityId { get; private set; }

        public int PageNumber { get; private set; }


        public int RowsPerPage { get; private set; }

        public string Term { get; private set; }
    }
}
