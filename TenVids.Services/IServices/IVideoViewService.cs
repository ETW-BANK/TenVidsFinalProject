
namespace TenVids.Services.IServices
{
    public interface IVideoViewService
    {
        Task HandleVideoViewAsync(string userId, int videoId, string ipAddress);
    }
}
