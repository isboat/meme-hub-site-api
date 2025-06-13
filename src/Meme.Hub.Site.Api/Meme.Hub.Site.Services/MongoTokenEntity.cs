namespace Meme.Hub.Site.Services
{
    public class MongoTokenEntity
    {
        public string Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string StrValue { get; set; }
        public DateTime ExpiresOn { get; internal set; }
    }
}