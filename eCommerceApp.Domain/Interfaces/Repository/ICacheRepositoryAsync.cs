namespace eCommerceApp.Domain.Interfaces.Repository
{
    public interface ICacheRepositoryAsync
    {
        Task<string> ClearCache(CancellationToken cancellationToken = default);
    }
}
