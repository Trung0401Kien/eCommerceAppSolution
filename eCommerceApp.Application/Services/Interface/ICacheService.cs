namespace eCommerceApp.Application.Services.Interface
{
    public interface ICacheService
    {
        Task<T?> GetSessionAsync<T>(string sessionId);
        Task<bool> DeleteSessionAsync(string sessionId);
        Task<bool> CreateNewSessionAsync(string userId, string sessionId);
    }
}
