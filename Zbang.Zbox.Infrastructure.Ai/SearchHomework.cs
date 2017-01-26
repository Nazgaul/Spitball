using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class SearchHomework : SearchDocumentIntent
    {
        public override ItemType? TypeToSearch => ItemType.Homework | ItemType.Document;
    }

    public class SearchLecture : SearchDocumentIntent
    {
        public override ItemType? TypeToSearch => ItemType.Lecture | ItemType.Document;
    }
    public class SearchStudyGuide : SearchDocumentIntent
    {
        public override ItemType? TypeToSearch => ItemType.StudyGuide | ItemType.Document;
    }
    public class SearchQuiz : SearchDocumentIntent
    {
        public override ItemType? TypeToSearch => ItemType.Quiz;
    }
    public class SearchClassNote : SearchDocumentIntent
    {
        public override ItemType? TypeToSearch => ItemType.ClassNote | ItemType.Document;
    }
    public class SearchFlashcard : SearchDocumentIntent
    {
        public override ItemType? TypeToSearch => ItemType.Flashcard;
    }
   
}