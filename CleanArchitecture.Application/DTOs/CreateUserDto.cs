﻿namespace CleanArchitecture.Application.DTOs;

public class CreateUserDto
{    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
}