using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategory", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("Id");      
        builder.Property(x => x.Description).HasColumnName("Description");
    }
}
