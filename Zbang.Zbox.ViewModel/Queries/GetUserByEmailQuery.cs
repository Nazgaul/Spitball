using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserByEmailQuery
    {
        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
        public string Email { get; private set; }
    }
}
