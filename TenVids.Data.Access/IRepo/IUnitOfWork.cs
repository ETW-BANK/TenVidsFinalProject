namespace TenVids.Data.Access.IRepo
{
    public interface IUnitOfWork: IDisposable
    {
        IChannelRepository ChannelRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IVideosRepository VideosRepository { get; }
        IVideoFileRepository VideoFileRepository { get; }
        ICommentsRepository CommentsRepository { get; }
        IVideoViewRepository VideoViewRepository { get; }
        ILikesRepository LikesRepository { get; }
        Task <bool> CompleteAsync();
    }
}
