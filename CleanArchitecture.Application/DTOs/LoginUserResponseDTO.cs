namespace CleanArchitecture.Application.DTOs;

public class LoginUserResponseDTO
{
    public string AccessToken { get; set; }    
    public DateTime ExpiresAt { get; set; }
}