using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class UserWithoutUniversityQuery : IPagedQuery
    {
        public UserWithoutUniversityQuery(int pageNumber, int rowsPerPage)
        {
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
        }

        public int PageNumber { get; }
        public int RowsPerPage { get; }
    }
}
