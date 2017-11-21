using System;
using Microsoft.EntityFrameworkCore;
using Cloudents.Core.Entities.Db;

namespace Cloudents.Infrastructure.Data
{
    public partial class SpitballContent : DbContext
    {
        //public SpitballContent()
        //    : base("name=CourseContent")
        //{
        //}

        public SpitballContent(DbContextOptions<SpitballContent> options) :base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<University> Universities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                
            }
        }
        //https://putshello.wordpress.com/2014/08/20/entity-framework-soft-deletes-are-easy/
        //TODO : need to do soft delete and isDirty
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255).HasColumnName("BoxName");
                entity.Property(e => e.Id).HasColumnName("BoxId");
                entity.Property(e => e.CreationTime).ValueGeneratedOnAdd().HasDefaultValue(DateTime.Now);
                entity.Property(e => e.UpdateTime).ValueGeneratedOnAddOrUpdate().HasDefaultValue(DateTime.Now);
                entity.ToTable("Box", "Zbox");
                entity.HasQueryFilter(e => !e.IsDeleted).Ignore(e => e.IsDeleted);
                entity.Property(e => e.IsDirty).ValueGeneratedOnAddOrUpdate().HasDefaultValue(true);
                entity.Property(e => e.CourseCode).HasMaxLength(255);
            });

            modelBuilder.Entity<University>(entity =>
            {
                entity.HasMany(e => e.Courses);
                entity.Property(e => e.Extra).HasMaxLength(255);
                entity.Property(e => e.Image).HasColumnName("LargeImage").HasMaxLength(255);
                entity.Property(e => e.Name).HasColumnName("UniversityName").HasMaxLength(255);
                entity.Property(e => e.Extra).HasMaxLength(255);
                entity.Property(e => e.Country).HasMaxLength(2);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.CreationTime).ValueGeneratedOnAdd().HasDefaultValue(DateTime.Now);
                entity.Property(e => e.UpdateTime).ValueGeneratedOnAddOrUpdate().HasDefaultValue(DateTime.Now);
                entity.HasQueryFilter(e => !e.IsDeleted).Ignore(e => e.IsDeleted);
                entity.Property(e => e.IsDirty).ValueGeneratedOnAddOrUpdate().HasDefaultValue(true);
                entity.ToTable("University", "Zbox");
            });


            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            //ChangeTracker.Entries().Where(p=>p.State == )
            return base.SaveChanges();
        }
        //  protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //  {

        //  }
    }
}
