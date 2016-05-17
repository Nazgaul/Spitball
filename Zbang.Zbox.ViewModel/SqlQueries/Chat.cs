using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Chat
    {
        public const string MessageBoard = @"select userid as id,username as name, UserImageLarge as image, online 
from zbox.users u where userid in @UserIds";
    }
}
