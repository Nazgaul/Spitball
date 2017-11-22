using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cloudents.Core.Entities.Db;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace Cloudents.Infrastructure.Data
{
    public partial class AppDbContext : DbContext
    {
        //public SpitballContent()
        //    : base("name=CourseContent")
        //{
        //}

        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] { new DebugLoggerProvider() });

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<University> Universities { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //if (!optionsBuilder.IsConfigured)
        //    //{
        //        optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        //    //}
        //}
        //https://putshello.wordpress.com/2014/08/20/entity-framework-soft-deletes-are-easy/
        //TODO : need to do soft delete and isDirty
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RowDetailsConfiguration());
            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255).HasColumnName("BoxName");
                entity.Property(e => e.Id).HasColumnName("BoxId");
                entity.ToTable("Box", "Zbox");
                entity.HasQueryFilter(e => !e.IsDeleted).Ignore(e => e.IsDeleted);
                entity.Property(e => e.IsDirty);//.ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.CourseCode).HasMaxLength(255);

                entity.OwnsOne(e => e.RowDetail, p =>
                {
                    p.Property(z => z.CreationTime).HasColumnName("CreationTime");
                    p.Property(z => z.UpdateTime).HasColumnName("UpdateTime");
                    p.Property(z => z.CreatedUser).HasColumnName("CreatedUser");
                    p.Property(z => z.UpdatedUser).HasColumnName("UpdatedUser");
                });
                entity.Property(p => p.UniversityId).HasColumnName("University");
                entity.HasOne(e => e.University).WithMany(b => b.Courses).HasForeignKey(e=>e.UniversityId);
            });

            modelBuilder.Entity<University>(entity =>
            {
                //entity.HasMany(e => e.Courses).HasForeignKey;
                entity.Property(e => e.Extra).HasMaxLength(255);
                entity.Property(e => e.Image).HasColumnName("LargeImage").HasMaxLength(255);
                entity.Property(e => e.Name).HasColumnName("UniversityName").HasMaxLength(255);
                entity.Property(e => e.Extra).HasMaxLength(255);
                entity.Property(e => e.Country).HasMaxLength(2);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasQueryFilter(e => !e.IsDeleted).Ignore(e => e.IsDeleted);
                entity.Property(e => e.IsDirty);//.ValueGeneratedOnAddOrUpdate().HasDefaultValue(true);
                entity.OwnsOne(e => e.RowDetail, p =>
                {
                    p.Property(z => z.CreationTime).HasColumnName("CreationTime");
                    p.Property(z => z.UpdateTime).HasColumnName("UpdateTime");
                    p.Property(z => z.CreatedUser).HasColumnName("CreatedUser");
                    p.Property(z => z.UpdatedUser).HasColumnName("UpdatedUser");
                });

                entity.ToTable("University", "Zbox");
            });


            base.OnModelCreating(modelBuilder);
        }

        

        public override int SaveChanges()
        {
            //ChangeTracker.Entries().Where(p=>p.State == )
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries().Where(p => p.State == EntityState.Added))
            {
                if (entry.Entity is Course b)
                {
                    b.IsDeleted = false;
                    b.IsDirty = true;
                    //b.CreationTime = DateTime.UtcNow;
                    //b.UpdateTime = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        //  protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //  {

        //  }
    }
}
