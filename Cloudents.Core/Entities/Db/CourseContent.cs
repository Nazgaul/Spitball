namespace Cloudents.Core.Entities.Db
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CourseContent : DbContext
    {
        public CourseContent()
            : base("name=CourseContent")
        {
        }

        public virtual DbSet<Box> Boxes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
