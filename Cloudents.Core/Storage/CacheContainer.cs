namespace Cloudents.Core.Storage
{
    public class CacheContainer : IStorageContainer
    {
        public StorageContainer Container  => StorageContainer.CacheContainer;
    }

    public class SpitballContainer : IStorageContainer
    {
        public StorageContainer Container =>StorageContainer.SpitballContainer;
    }

    public class QuestionAnswerContainer : IStorageContainer
    {
        public StorageContainer Container =>StorageContainer.QuestionsAndAnswers;
    }
}