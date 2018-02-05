using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Framework.Database
{
    public class UniversityMap : ClassMap<University>
    {
        public UniversityMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.IsDeleted);
            /*entity.Property(e => e.Extra).HasMaxLength(255);
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

                entity.ToTable("University", "Zbox");*/
            //Table("University");
            Schema("Zbox");

        }
    }
}