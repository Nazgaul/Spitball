﻿using System;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class ItemDto
    {
        public ItemDto()
        {
            
        }
        //version 2 api mobile
        public ItemDto(Int64 id, String name, Int64 ownerId,
            Guid? questionId, Guid? answerId, String type, String source)
            : this(id, name, ownerId, null, questionId, answerId, null, type, source)
        {

        }

       
        //this is for item
        public ItemDto(Int64 id, String name, Int64 ownerId, String thumbnail,
             Guid? questionId, Guid? answerId, String url, String type, String source)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            Thumbnail = thumbnail;
            QuestionId = questionId;
            AnswerId = answerId;
            Url = url;
            Type = type;
            Source = source;
        }
        //this is for question
        public ItemDto(Int64 id, String name, Int64 ownerId,
             Guid? questionId, String url)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            QuestionId = questionId;
            Url = url;
        }

        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Thumbnail { get; set; }
        public long OwnerId { get; private set; }

        public Guid? QuestionId { get; set; }
        public Guid? AnswerId { get; set; }

        public string Url { get; set; }

        public string DownloadUrl { get; set; }
        public string Type { get; set; }

        public string Source { get; set; }
    }
}
