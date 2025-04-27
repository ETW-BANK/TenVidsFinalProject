using TenVids.Models;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services.IServices
{
    public interface ICommentService
    {
      Task<ErrorModel<Comment>> CreateCommentsAsync(CommentsVM commentsVM);
    }
}
