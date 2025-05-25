namespace CleanArchitecture.Application.DTOs;
public class UpdateProductDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
