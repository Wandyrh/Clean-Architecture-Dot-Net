using CleanArchitecture.Domain.Entities.BaseEntities;

namespace CleanArchitecture.Domain.Entities;

public class ProductCategory : BaseEntity
{   
    public string Name { get; set; }
    public string Description { get; set; }    

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
