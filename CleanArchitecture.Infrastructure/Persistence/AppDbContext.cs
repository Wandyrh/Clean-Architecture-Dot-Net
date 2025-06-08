using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CleanArchitecture.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ProductConfiguration());
        builder.ApplyConfiguration(new ProductCategoryConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "_");
                var filter = Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted)),
                Expression.Constant(false)
                ),
                    parameter
                );

                builder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }
}
