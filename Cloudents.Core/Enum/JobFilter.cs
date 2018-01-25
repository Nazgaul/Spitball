using System;

namespace Cloudents.Core.Enum
{
    public enum JobFilter
    {
        None,
        [Parse("Full Time")]
        FullTime,
        [Parse("Part Time")]
        PartTime,
        [Parse("Internship")]
        Internship,
        [Parse("Campus Rep")]
        CampusRep,
        [Parse("Contractor")]
        Contractor,
        [Parse("Temporary")]
        Temporary,
        [Parse("Remote")]
        Remote
    }

    [Flags]
    public enum TutorFilter
    {
        None = 0,
        Online = 1,
        InPerson = 2,
    }
}