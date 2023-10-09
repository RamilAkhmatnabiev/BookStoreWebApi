using BookStore.Services.Interfaces;
using BookStoreWebApi.Models;

namespace BookStore.Extensions;

public static class UserExtensions
{
    public static IServiceProvider _serviceProvider;

    public static UserWithToken ToUserWithTokenWithAutoTokenCreate(this User user)
    {
        var tokenHelper = _serviceProvider.GetService<IUserTokenHelper>();
        var token = tokenHelper.CreateToken(user);

        return user.ToUserWithToken(token);
    }
    public static UserWithToken ToUserWithToken(this User user, string? token = null)
    {
        return new UserWithToken(user, token);
    }
}