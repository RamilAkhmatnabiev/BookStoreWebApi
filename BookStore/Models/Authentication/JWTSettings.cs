namespace BookStoreWebApi.Models;

public record JWTSettings
{
    public string SecretKey {get; set;} 
    public string Issuer {get; set;} 
    public string Audience {get; set;} 
    public int ExpirationSeconds {get; set;} 
}