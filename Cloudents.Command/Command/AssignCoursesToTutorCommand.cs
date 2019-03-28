using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command
{
    public class AssignCoursesToTutorCommand: ICommand
    { 
    public AssignCoursesToTutorCommand(IEnumerable<string> name, long userId)
    {
        Name = name;
        UserId = userId;
    }


    public IEnumerable<string> Name { get; }
    public long UserId { get; }
  
    }
}
