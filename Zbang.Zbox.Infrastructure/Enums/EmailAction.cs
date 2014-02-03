using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum EmailAction
    {
        None = 0,
        [EnumDescription(typeof(EnumResources), "EmailActionAddedItem")]
        AddedItem,
        //[EnumDescription(typeof(EnumResources), "EmailActionCommentItem")]
        //CommentItem,
        [EnumDescription(typeof(EnumResources), "EmailActionCommentBox")]
        AskedQuestion,
        [EnumDescription(typeof(EnumResources), "EmailActionJoin")]
        Join,
        [EnumDescription(typeof(EnumResources), "EmailActionAnswerBox")]
        Answered
    }
}
