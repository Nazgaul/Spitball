using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs.Emails
{
    public class PartnersDto
    {
        public PartnersDto()
        {
            Univeristies = new List<Univeristy>();
        }
        public int LastWeekUsers { get; set; }
        public int AllUsers { get; set; }

        public int LastWeekCourses { get; set; }
        public int AllCourses { get; set; }

        public int LastWeekItems { get; set; }
        public int AllItems { get; set; }

        public int LastWeekQnA { get; set; }
        public int AllQnA { get; set; }

        public IEnumerable<Univeristy> Univeristies { get; set; }
    }

    public class Univeristy
    {
        public string Name { get; set; }
        public int Students { get; set; }
    }


}
