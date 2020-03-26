using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    //public class NewCourseDto
    //{
    //    [EntityBind(nameof(Course.Id))]
    //    public string NewCourse { get; set; }
    //    [EntityBind(nameof(Course.Id))]
    //    public string OldCourse { get; set; }
    //}


    public class PendingCoursesDto
    {
        [EntityBind(nameof(Course.Id))]
        public string Name { get; set; }
    }



    //public class PendingUniversitiesDto
    //{
    //    [EntityBind(nameof(University.Id))]
    //    public Guid Id { get; set; }
    //    [EntityBind(nameof(University.Name))]
    //    public string Name { get; set; }
    //    [EntityBind(nameof(University.RowDetail.CreationTime))]
    //    public DateTime Created { get; set; }
    //    public bool CanBeDeleted { get; set; }
    //}

    //public class AllUniversitiesDto
    //{
    //    [EntityBind(nameof(University.Id))]
    //    public Guid Id { get; set; }
    //    [EntityBind(nameof(University.Name))]
    //    public string Name { get; set; }
    //}
}
