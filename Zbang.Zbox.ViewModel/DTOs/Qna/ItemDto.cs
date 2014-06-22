﻿using System;

namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public abstract class ItemDto
    {
        protected ItemDto(long uid, long ownerId,
            string boxUid, Guid? questionId, Guid? answerId, string name, string url)
        {
            Uid = uid;
            OwnerId = ownerId;
            BoxUid = boxUid;
            QuestionId = questionId;
            AnserId = answerId;
            Name = name;
            Url = url;
        }
        public long Uid { get; private set; }
        public string Name { get; private set; }
        public string Thumbnail { get; set; }
        public string BoxUid { get; private set; }
        public long OwnerId { get; private set; }
        public abstract string Type { get; }

        public Guid? QuestionId { get; set; }
        public Guid? AnserId { get; set; }

        public string Url { get; set; }
    }
}
