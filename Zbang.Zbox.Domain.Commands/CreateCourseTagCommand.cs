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
            if (!string.IsNullOrWhiteSpace(code))
            {
                Code = code;
            }
            if (!string.IsNullOrWhiteSpace(professor))
            {
                Professor = professor;
            }
        }

        public string Name { get; private set; }
        public string Code { get; private set; }
        public string Professor { get; private set; }
    }
}
