using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddStudentCommand : ICommand
    {
        public AddStudentCommand(string id)
        {
            Id = id;
        }
        public string Id { get; set; }
    }
}
