﻿using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddCommentCommandResult : ICommandResult
    {
        public AddCommentCommandResult(Guid commentId, string userName, string userImage, string userUrl, long userId)
        {
            UserId = userId;
            UserUrl = userUrl;
            UserImage = userImage;
            UserName = userName;
            CommentId = commentId;
        }

        public string UserName { get; private set; }
        public string UserImage { get; private set; }
        public Guid CommentId { get; private set; }
        public string UserUrl { get; private set; }
        public long UserId { get; private set; }

        public override string ToString()
        {
           return string.Format("userid {0} user url {1} userimage {2} username {3} commentid {4}", UserId, UserUrl, UserImage,
                UserName, CommentId);
        }
    }
}
