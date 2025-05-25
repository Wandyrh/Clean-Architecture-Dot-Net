namespace CleanArchitecture.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ProductCategory Category { get; set; }
}
