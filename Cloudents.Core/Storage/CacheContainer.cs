namespace Cloudents.Core.Storage
{
    //public class OldCacheContainer : IStorageContainer
    //{
    //    public StorageContainer Container => StorageContainer.OldCacheContainer;
    //}

    public class SpitballContainer : IStorageContainer
    {
        public StorageContainer Container => StorageContainer.SpitballContainer;
    }

    public class QuestionAnswerContainer : IStorageContainer
    {
        public StorageContainer Container => StorageContainer.QuestionsAndAnswers;
    }

    public class DocumentContainer : IStorageContainer
    {
        public StorageContainer Container => StorageContainer.Document;
    }

    //public class IcoContainer : IStorageContainer
    //{
    //    public StorageContainer Container => StorageContainer.IcoFiles;
    //}
}