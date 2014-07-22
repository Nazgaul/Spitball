using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Dto.ActivityDtos
{
    public class CommentDto : BaseActivityDto, IComposite<CommentDto>
    {
        public CommentDto(long commentId, long? parentId, string userName, string userImg, long userUid, DateTime date,
            string boxName, string boxUid,
            long? itemUid, string itemName, string comment)
            : base(userName, userImg, userUid, date, boxName, boxUid,
                 itemUid, itemName)
        {
            Comment = comment;
            Id = commentId;
            ParentId = parentId;
            Replies = new List<CommentDto>();

        }

        public string Comment { get; private set; }

        public long Id { get; private set; }
        public long? ParentId { get; private set; }

        public List<CommentDto> Replies { get; set; }


    }
}
