using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Cloudents.Command.Command
{
    public class CreateDocumentCommand : ICommand
    {
        public CreateDocumentCommand(string blobName, string name, string type, string course,
            IEnumerable<string> tags, long userId, string professor, decimal modelPrice)
        {
            BlobName = blobName;
            Name = name;
            Type = type;
            Course = course;
            Tags = tags ?? Enumerable.Empty<string>();
            Professor = professor;
            Price = modelPrice;
            UserId = userId;
        }

        public string BlobName { get; }
        [NotNull] public string Name { get; }
        public string Type { get; }

        public string Course { get; }
        public IEnumerable<string> Tags { get; }

        public long UserId { get; }

        public string Professor { get; set; }
        public decimal Price { get; }

        public long Id { get; set; }
    }
}