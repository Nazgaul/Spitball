using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddNodeToLibraryCommand : ICommand
    {
        public AddNodeToLibraryCommand(string name, long universityId , Guid? parentId, long userId)
        {
            UserId = userId;
            if (name == null) throw new ArgumentNullException("name");

            Name = name;
            UniversityId = universityId;
            ParentId = parentId;
        }
        public string Name { get; private set; }
        public long UniversityId { get; private set; }
        public Guid? ParentId { get; private set; }
        public long UserId { get; private set; }


        public Guid Id { get; set; }
        public string Url { get; set; }
    }
}
