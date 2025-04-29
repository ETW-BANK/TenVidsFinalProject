

using TenVids.Models.DTOs;

namespace TenVids.Services.IServices
{
    public interface ISideBarService
    {

        Task<IEnumerable<SubscriptionDto>> GetSubscriptions();

        Task<IEnumerable<HistoryDto>> GetHistory();
    }
}
