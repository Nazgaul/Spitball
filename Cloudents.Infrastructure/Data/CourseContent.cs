using Microsoft.EntityFrameworkCore;
using Cloudents.Core.Entities;

namespace Cloudents.Infrastructure.Data
{
    public partial class CourseContent : DbContext
    {
        //public CourseContent()
        //    : base("name=CourseContent")
        //{
        //}

        //public virtual DbSet<Course> Courses { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;");
//            }
//        }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {

//        }
    }
}
