
namespace TenVids.Repository.IRepository
{
    public interface IUnitOfWork: IDisposable
    {
        IChannelRepository ChannelRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IVideosRepository VideosRepository { get; }

        Task <bool> CompleteAsync();
    }
}
