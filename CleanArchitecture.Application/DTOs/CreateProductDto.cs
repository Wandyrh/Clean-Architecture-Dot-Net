namespace CleanArchitecture.Application.DTOs;

public class CreateProductDto
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
