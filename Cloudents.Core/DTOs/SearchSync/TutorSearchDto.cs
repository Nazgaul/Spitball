using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class TutorSearchDto
    {
       

        public long Id { get; set; } //key readonly

        public string Name { get; set; } //key readonly
        public string Bio { get; set; } //key readonly
        public string Image { get; set; } //key readonly
        public double Price { get; set; } //key readonly
        public double Rate { get; set; } //key readonly
        public int ReviewsCount { get; set; } //key readonly
        public byte[] Version { get; set; } //key readonly
        public IList<string> Courses { get; set; } //key readonly
        public IList<string> Subjects { get; set; } //key readonly

        public long SYS_CHANGE_VERSION { get; set; }
        public string SYS_CHANGE_OPERATION { get; set; }

        public ItemState State { get; set; }

        public long VersionAsLong => BitConverter.ToInt64(Version.Reverse().ToArray(), 0);
    }


    public class TutorSearchWrapperDto : SearchWrapperDto<TutorSearchDto>
    {
        //public stirng Type { get; set; }
    }
}