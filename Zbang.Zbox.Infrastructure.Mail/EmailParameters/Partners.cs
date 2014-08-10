using System.Collections.Generic;
using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail.EmailParameters
{
    public class Partners : MailParameters
    {
        public override string MailResover
        {
            get { return PartnersResolver; }
        }

        public Partners(CultureInfo culture,
            string schoolName, 
            int weekUsers,
            int allUsers,
            int weekCourses,
            int allCourses,
            int weekItems,
            int allItems,
            int weekQnA,
            int allQnA,
            IEnumerable<University> universities
            )
            :base(culture)
        {
            SchoolName = schoolName;
            WeekUsers = weekUsers;
            AllUsers = allUsers;
            WeekCourses = weekCourses;
            AllCourses = allCourses;
            WeekItems = weekItems;
            AllItems = allItems;
            WeekQnA = weekQnA;
            AllQnA = allQnA;
            Universities = universities;
        }

        public string SchoolName { get; private set; }

        public int WeekUsers { get; private set; }
        public int AllUsers { get; private set; }

        public int WeekCourses { get; private set; }
        public int AllCourses { get; private set; }

        public int WeekItems { get; private set; }
        public int AllItems { get; private set; }

        public int WeekQnA { get; private set; }
        public int AllQnA { get; private set; }

        public IEnumerable<University> Universities { get; set; }

        public class University
        {
            public int StudentsCount { get; set; }
            public string Name { get; set; }
        }
    }
}
