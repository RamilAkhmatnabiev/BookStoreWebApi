namespace BookStoreWebApi.Models;

public class UserWithToken : User
{
    public UserWithToken(User user, string? token = null)
    {
        this.UserId = user.UserId;
        this.EmailAddress = user.EmailAddress;            
        this.FirstName = user.FirstName;
        this.MiddleName = user.MiddleName;
        this.LastName = user.LastName;
        this.PubId = user.PubId;
        this.HireDate = user.HireDate;
        this.Role = user.Role;

        this.Token = token;
    }
    
    public string? Token { get; set; }
}