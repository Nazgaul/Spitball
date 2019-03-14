using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class NewCourseDto
    {
        [DtoToEntityConnection(nameof(Course.Name))]
        public string NewCourse { get; set; }
        [DtoToEntityConnection(nameof(Course.Name))]
        public string OldCourse { get; set; }
    }


    public class PendingCoursesDto
    {
        [DtoToEntityConnection(nameof(Course.Name))]
        public string Name { get; set; }
        [DtoToEntityConnection(nameof(Course.Created))]
        public DateTime Created { get; set; }
    }

        public class NewUniversitiesDto
    {
        [DtoToEntityConnection(nameof(University.Id))]
        public Guid NewId{ get; set; }
        [DtoToEntityConnection(nameof(University.Name))]
        public string NewUniversity { get; set; }
        [DtoToEntityConnection(nameof(University.Id))]
        public Guid OldId { get; set; }
        [DtoToEntityConnection(nameof(University.Name))]
        public string OldUniversity { get; set; }

    }


    public class PendingUniversitiesDto
    {
        [DtoToEntityConnection(nameof(University.Id))]
        public Guid Id { get; set; }
        [DtoToEntityConnection(nameof(University.Name))]
        public string Name { get; set; }
        [DtoToEntityConnection(nameof(University.RowDetail.CreationTime))]
        public DateTime Created { get; set; }
    }

    public class AllUniversitiesDto
    {
        [DtoToEntityConnection(nameof(University.Id))]
        public Guid Id { get; set; }
        [DtoToEntityConnection(nameof(University.Name))]
        public string Name { get; set; }
    }
    }
