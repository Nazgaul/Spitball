namespace Cloudents.Core.Storage
{

    public class QuestionAnswerContainer : IStorageContainer
    {
        public StorageContainer Container => StorageContainer.QuestionsAndAnswers;
    }

    public class DocumentContainer : IStorageContainer
    {
        public StorageContainer Container => StorageContainer.Document;
    }


}