﻿namespace CleanArchitecture.Application.DTOs;
public class ProductDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
}
