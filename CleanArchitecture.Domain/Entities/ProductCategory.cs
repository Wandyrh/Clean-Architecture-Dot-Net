namespace CleanArchitecture.Domain.Entities;

public class ProductCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }    

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
