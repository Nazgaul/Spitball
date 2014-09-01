using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateDepartmentCommand : ICommand
    {
        public CreateDepartmentCommand(string name, long universityId)
        {
            Name = name;
            UniversityId = universityId;
        }

        public string Name { get; private set; }
        public long UniversityId { get; private set; }


        public long Id { get; set; }
    }

}
