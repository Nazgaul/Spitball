using System;
using System.Collections.Generic;
using System.Text;
using Cloudents.Core.Entities.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloudents.Infrastructure.Data
{
    internal class RowDetailsConfiguration : IEntityTypeConfiguration<RowDetail>
    {
        public void Configure(EntityTypeBuilder<RowDetail> builder)
        {
            builder.Property(e => e.CreationTime).HasColumnName("CreationTime").IsRequired();//.ValueGeneratedOnAdd().HasDefaultValueSql("getUtcDate()");
            builder.Property(e => e.UpdateTime).HasColumnName("UpdateTime").IsRequired();
            builder.Property(e => e.CreatedUser).HasColumnName("CreatedUser").HasMaxLength(255).IsRequired();
            builder.Property(e => e.UpdatedUser).HasColumnName("UpdatedUser").HasMaxLength(255).IsRequired();
        }
    }
}
