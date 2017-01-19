using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateCourseTagCommand : ICommand
    {
        public CreateCourseTagCommand(string name, string code, string professor)
        {
            Name = name;
            Code = code;
            Professor = professor;
        }

        public string Name { get; private set; }
        public string Code { get; private set; }
        public string Professor { get; private set; }
    }
}
