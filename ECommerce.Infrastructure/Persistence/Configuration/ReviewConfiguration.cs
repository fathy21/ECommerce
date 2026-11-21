using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configuration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.CustomerId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            builder.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Reviews_Rating",
                    "[Rating] >= 1 AND [Rating] <= 5"));

            builder.HasIndex(r => new { r.CustomerId, r.ProductId })
                .IsUnique();
        }
    }
}
