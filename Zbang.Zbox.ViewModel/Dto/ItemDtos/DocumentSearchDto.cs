﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class DocumentSearchDto : ItemSearchDto
    {
        private static readonly string[] ClassNotes = { "Classnotes", "class note", "note","notes" };
        private static readonly string[] Homework = { "Homework", "hw", "home work", "assignments", "assignment" };
        private static readonly string[] Lectures = { "Lectures", "lecture" };
        private static readonly string[] StudyGuides = { "Study Guides", "study guide", "review", "guide", "reviews", "guides" };
        private static readonly string[] Exams = { "Tests & Exams", "exam", "test", "midterm", "final" };

        private static bool ClassifyType(string[] words, string name, string tab)
        {
            return
                words.Any(c => tab?.IndexOf(c, StringComparison.InvariantCultureIgnoreCase) >= 0)
                ||
                words.Any(c => name?.IndexOf(c, StringComparison.InvariantCultureIgnoreCase) >= 0);
            //||
            //words.Any(c => content?.IndexOf(" " + c + " ", StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        private IEnumerable<ItemType> m_Type;
        public string Image { get; set; }

        // public override string Content { get; set; }
        public string DocumentContent { get; set; }
        public override string Content => DocumentContent;

        public override IEnumerable<ItemType> Type
        {
            get
            {
                if (m_Type != null && m_Type.Any())
                {
                    return m_Type;
                }
                var type = new List<ItemType>
                {
                    TypeDocument.ToLowerInvariant() == "file" ? ItemType.Document : ItemType.Link
                };
                if (string.IsNullOrEmpty(TabName) && string.IsNullOrEmpty(Content))
                {
                    return type;
                }
                if (ClassifyType(ClassNotes, Name, TabName))
                {
                    type.Add(ItemType.ClassNote);
                }
                if (ClassifyType(Homework, Name, TabName))
                {
                    type.Add(ItemType.Homework);
                }
                if (ClassifyType(StudyGuides, Name, TabName))
                {
                    type.Add(ItemType.StudyGuide);
                }
                if (ClassifyType(Exams, Name, TabName))
                {
                    type.Add(ItemType.Exam);
                }
                if (ClassifyType(Lectures, Name, TabName))
                {
                    type.Add(ItemType.Lecture);
                }
                m_Type = type;
                return m_Type;
            }

        }

        public override string SearchContentId => "item_" + Id;

        public string Url { get; set; }

        public long? UniversityId { get; set; }

        public string TabName { get; set; }

        public string TypeDocument { get; set; }

        public IEnumerable<long> UserIds { get; set; }

        public override string ToString()
        {
            return $"id : {Id} blobName: {BlobName}";
        }

        public override string[] MetaContent
        {
            get
            {
                if (string.IsNullOrEmpty(Content))
                {
                    return null;
                }
                return new[] { Content.RemoveEndOfString(200) };
            }
        }

        
        public override int? ContentCount => null;
    }
}