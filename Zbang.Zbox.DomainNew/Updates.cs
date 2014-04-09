﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class Updates
    {
        protected Updates()
        {

        }
        public Updates(User user, Box box,
            Comment comment = null, CommentReplies reply = null, Item item = null,
            Quiz quiz = null
            )
        {
            User = user;
            Box = box;
            Comment = comment;
            Reply = reply;
            Item = item;
            Quiz = quiz;
            CreationTime = DateTime.UtcNow;
        }
       
        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual Box Box { get; set; }
        public virtual Comment Comment { get; set; }
        public virtual CommentReplies Reply { get; set; }
        public virtual Item Item { get; set; }
        public virtual Quiz Quiz { get; set; }
      
        public virtual DateTime CreationTime { get; set; }
    }
}
