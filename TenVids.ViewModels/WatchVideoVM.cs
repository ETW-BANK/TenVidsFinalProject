
namespace TenVids.ViewModels
{
    public class WatchVideoVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public bool IsSubscribed { get; set; }
       public DateTime CreatedAt { get; set; } 
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public int ViewsCount { get; set; }
        public int SubscribersCount { get; set; }   


        public int Views { get; set; }
        public DateTime UploadDate { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        //public List<CommentVM> Comments { get; set; } = new List<CommentVM>();

    }
}
