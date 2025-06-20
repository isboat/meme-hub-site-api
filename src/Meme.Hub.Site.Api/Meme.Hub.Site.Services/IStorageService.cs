namespace Meme.Hub.Site.Services
{
    public interface IStorageService
    {
        Task<bool> RemoveAsync(string tokenAddr, string fileName);
        Task<string> UploadAsync(string tokenAddr, string fileName, Stream stream);
    }
}