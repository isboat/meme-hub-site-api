using Meme.Domain.Models.TokenModels;

namespace Meme.Hub.Site.Services
{
    public interface ICacheService
    {

        Task<List<TokenDataModel>> GetItemsFromList();

        Task RemoveExpiredItemsAsync();

        T? GetData<T>(string key);
    }
}