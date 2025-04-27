using Microsoft.AspNetCore.Http;
using TenVids.Models;
using TenVids.Repository.IRepository;
using TenVids.Services.Extensions;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services
{
    public  class CommentService: ICommentService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public CommentService(IUnitOfWork unitOfWork,IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ErrorModel<Comment>> CreateCommentsAsync(CommentsVM commentsVM)
        {
            try
            {
                // Validate input
                if (commentsVM?.PostComment == null)
                {
                    return new ErrorModel<Comment>
                    {
                        IsSuccess = false,
                        Message = "Invalid comment data",
                        Data = null
                    };
                }

                // Get current user
                var userId = _httpContextAccessor.HttpContext.User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return new ErrorModel<Comment>
                    {
                        IsSuccess = false,
                        Message = "User not authenticated",
                        Data = null
                    };
                }

                // Validate content
                var content = commentsVM.PostComment.Content?.Trim();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return new ErrorModel<Comment>
                    {
                        IsSuccess = false,
                        Message = "Comment content cannot be empty",
                        Data = null
                    };
                }

                
                var videoExists = await _unitOfWork.VideosRepository.AnyAsync(v => v.Id == commentsVM.PostComment.VideoId);
                if (!videoExists)
                {
                    return new ErrorModel<Comment>
                    {
                        IsSuccess = false,
                        Message = "Video not found",
                        Data = null
                    };
                }

                var comment = new Comment
                {
                    AppUserId = userId,
                    VideoId = commentsVM.PostComment.VideoId,
                    Content = content,
                    PostedAt = DateTime.UtcNow
                };

                 _unitOfWork.CommentsRepository.Add(comment);
                await _unitOfWork.CompleteAsync();

                return new ErrorModel<Comment>
                {
                    IsSuccess = true,
                    Message = "Comment added successfully",
                    Data = comment
                };
            }
            catch (Exception ex)
            {
                
                   commentsVM.PostComment.Content = ex.Message; 

                return new ErrorModel<Comment>
                {
                    IsSuccess = false,
                    Message = "An error occurred while adding the comment",
                    Data = null
                };
            }
        }


    }

}
