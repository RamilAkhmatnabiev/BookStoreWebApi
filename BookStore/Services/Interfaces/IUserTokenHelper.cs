using BookStoreWebApi.Models;

namespace BookStore.Services.Interfaces;

public interface IUserTokenHelper
{
    void ApplyJWTToken(UserWithToken userWithToken);
    UserWithToken GetUserWithAppliedJWTToken(User user);
    public string CreateToken(User user);
}