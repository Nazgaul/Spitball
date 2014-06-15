using System;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddNodeToLibraryCommand : ICommand
    {
        public AddNodeToLibraryCommand(string name, Guid newId, long universityId , Guid? parentId)
        {
            Throw.OnNull(name,"name");
            Throw.OnEmpty(newId, "NewId");

            Name = name;
            NewId = newId;
            UniversityId = universityId;
            ParetnId = parentId;
        }
        public string Name { get; private set; }
        public Guid NewId { get; private set; }
        public long UniversityId { get; private set; }
        public Guid? ParetnId { get; private set; }
    }
}
