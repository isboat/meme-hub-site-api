using Meme.Hub.Site.Models;

namespace Meme.Hub.Site.Services
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(User user);
        Task<User> GetUserByPrivyId(string privyId);
    }
}
