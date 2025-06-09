namespace CleanArchitecture.Application.Common.Models;

public class JWTOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
    public int ExpiryHours { get; set; }
}
