namespace TenVids.Repository.IRepository
{
    public interface IUnitOfWork: IDisposable
    {
        IChannelRepository ChannelRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IVideosRepository VideosRepository { get; }
        IVideoFileRepository VideoFileRepository { get; }
        ICommentsRepository CommentsRepository { get; }
        IVideoViewRepository VideoViewRepository { get; }
        Task <bool> CompleteAsync();
    }
}
