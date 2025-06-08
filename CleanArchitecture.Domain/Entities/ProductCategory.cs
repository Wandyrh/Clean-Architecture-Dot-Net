using CleanArchitecture.Domain.Entities.BaseEntities;

namespace CleanArchitecture.Domain.Entities;

public class ProductCategory : AuditableEntity
{   
    public string Name { get; set; }
    public string Description { get; set; }    

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
