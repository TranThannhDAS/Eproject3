namespace backend.Dao.Repository
{
    public interface IResponseCacheService
    {
        Task SetCacheResponseAsync(string cache, object response,TimeSpan timeOut);
        Task<string> GetCacheResponseAsync(string cache);
        Task RemoveCache(string pattern);
    }
}
