using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Command.Command
{
    public class CreateDocumentCommand : ICommand
    {
        public CreateDocumentCommand(string blobName, string name, string course,
            long userId, decimal modelPrice, string modelDescription)
        {
            BlobName = blobName;
            Name = name;
            Course = course;
            Price = modelPrice;
            ModelDescription = modelDescription;
            UserId = userId;
        }



        public string BlobName { get; }
        [NotNull] public string Name { get; }

        public string Course { get; }

        public long UserId { get; }

        public decimal Price { get; }
        public string ModelDescription { get; }

    }
}