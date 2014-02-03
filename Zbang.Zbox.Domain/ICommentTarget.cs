using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Zbang.Zbox.Domain
{
    public interface ICommentTarget
    {
        ICollection<Comment> Comments { get; }
        Comment AddComment(User author, string commentText);
    }
}
