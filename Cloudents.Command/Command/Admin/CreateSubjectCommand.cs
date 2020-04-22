using System;
using Cloudents.Core.Entities;

namespace Cloudents.Command.Command.Admin
{
    public class CreateSubjectCommand : ICommand
    {
        public string Name { get; }
        public Guid UserId { get; }
        public Country Country { get; }

        public CreateSubjectCommand(string name, Guid userId, Country country)
        {
            Name = name;
            UserId = userId;
            Country = country;
        }
      
    }
}
