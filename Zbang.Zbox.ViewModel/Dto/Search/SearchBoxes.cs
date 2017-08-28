using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.Search
{
    public class SearchBoxes
    {
        public SearchBoxes()
        {

        }

        public SearchBoxes(long id, string name,
            string professor, string courseCode,
            string url , string departmentId,
            int itemsCount, int membersCount,
             BoxType? type = null)
        {
            Id = id;
            Name = name;
            Professor = professor;
            CourseCode = courseCode;
            Url = url;
            DepartmentId = departmentId;
            Type = type;
            ItemCount = itemsCount;
            MembersCount = membersCount;
        }
        public string Name { get; set; }

        public string Professor { get; set; }
        public string CourseCode { get; set; }
        public long Id { get; set; }

        public string Url { get; set; }

        public string DepartmentId { get; set; }

        public BoxType? Type { get; set; }

        public int MembersCount { get; set; }
        public int ItemCount { get; set; }
    }
}
