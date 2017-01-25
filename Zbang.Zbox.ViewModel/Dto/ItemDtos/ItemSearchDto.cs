using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public abstract class ItemSearchDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public abstract string Content { get; }
        public string UniversityName { get; set; }
        public string BoxName { get; set; }
        public string BoxCode { get; set; }
        public string BoxProfessor { get; set; }

        public abstract ItemType Type { get; }
        public Language? Language { get; set; }

        public IEnumerable<ItemSearchTag> Tags { get; set; }

        public abstract string SearchContentId { get; }// => "item_" + Id;
    }

    public class DocumentSearchDto : ItemSearchDto
    {
        private static readonly string[] ClassNotes = { "Classnotes", "Class Notes", "summary" };
        private static readonly string[] Homework = { "Homework", "assignment", "home work", "essay", "handout", "thesis" };
        private static readonly string[] Lectures = { "Lectures", "slides", "class discussion", "speech", "talk", "handouts" };
        private static readonly string[] StudyGuides = { "Study Guides", "study guide", "review", "templates", "summary" };
        private static readonly string[] Exams = { "Tests & Exams", "exam", "tests", "midterms", "finals", "examination" };

        private static bool ClassifyType(string[] words, string name, string tab, string content)
        {
            return
                words.Any(c => name?.IndexOf(c, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    ||
                words.Any(c => tab?.IndexOf(c, StringComparison.InvariantCultureIgnoreCase) >= 0)
                   ||
                   words.Any(c => content?.IndexOf(" " + c + " ", StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        private ItemType m_Type;
        public string Image { get; set; }

        // public override string Content { get; set; }
        public string DocumentContent { get; set; }
        public override string Content => DocumentContent;

        public override ItemType Type
        {
            get
            {
                if (m_Type != ItemType.Undefined)
                {
                    return m_Type;
                }
                var type = ItemType.Undefined;
                if (TypeDocument.ToLowerInvariant() == "file")
                {
                    type = type | ItemType.Document;
                }
                else
                {
                    type = type | ItemType.Link;
                }
                if (string.IsNullOrEmpty(TabName) && string.IsNullOrEmpty(Content))
                {

                    return type;
                }

                //if (Content.IndexOf("Class Notes", StringComparison.InvariantCultureIgnoreCase) >= 0)
                //{
                //    return type | ItemType.ClassNote;
                //}
                if (ClassifyType(ClassNotes,Name, TabName, Content))
                //if (TabName.Equals("Class Notes", StringComparison.InvariantCultureIgnoreCase))
                {
                    m_Type = type | ItemType.ClassNote;
                    return m_Type;
                }
                if (ClassifyType(Homework, Name, TabName, Content))
                //if (TabName.Equals("Homework", StringComparison.InvariantCultureIgnoreCase))
                {
                    m_Type = type | ItemType.Homework;
                    return m_Type;
                }
                if (ClassifyType(StudyGuides, Name, TabName, Content))
                //if (TabName.Equals("Study Guides", StringComparison.InvariantCultureIgnoreCase))
                {
                    m_Type = type | ItemType.StudyGuide;
                    return m_Type;
                }
                if (ClassifyType(Exams, Name, TabName, Content))
                //if (TabName.Equals("Tests & Exams", StringComparison.InvariantCultureIgnoreCase))
                {
                    m_Type = type | ItemType.Exam;
                    return m_Type;
                }
                if (ClassifyType(Lectures, Name, TabName, Content))
                //if (TabName.Equals("Lectures", StringComparison.InvariantCultureIgnoreCase))
                {
                    m_Type = type | ItemType.Lecture;
                    return m_Type;
                }
                m_Type = type;
                return m_Type;
            }

        }

        public override string SearchContentId => "item_" + Id;

        public string Url { get; set; }

        public long? UniversityId { get; set; }

        public string BlobName { get; set; }

        public long BoxId { get; set; }

        public string TabName { get; set; }

        public string TypeDocument { get; set; }

        public IEnumerable<ItemSearchUsers> UserIds { get; set; }

        public override string ToString()
        {
            return $"id : {Id} blobName: {BlobName}";
        }
    }

    public class ItemSearchUsers
    {
        public long Id { get; set; }
    }

    public class ItemSearchTag
    {
        public string Name { get; set; }
    }
}