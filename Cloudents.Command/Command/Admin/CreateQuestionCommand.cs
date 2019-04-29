using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cloudents.Command.Command.Admin
{
    public class CreateQuestionCommand : ICommand
    {
        public CreateQuestionCommand(string courseName, Guid university,
            string text, [CanBeNull] IEnumerable<string> files, string country)
        {
            CourseName = courseName;
            Text = text;
            Files = files;
            Country = country;
            University = university;
        }

        public string CourseName { get;  }
        public string Text { get; }


        [CanBeNull]
        public IEnumerable<string> Files { get; }

        public  string Country { get;  }
        public Guid University { get;  }
    }
}