﻿

namespace TenVids.ViewModels
{
    public class MemberVM
    {
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public DateTime CreatedAt { get; set; }
        public int NumberOfSubscribers { get; set; }
        public int NumberOfAvailableVideos { get; set; }
        public bool UserIsSubscribed { get; set; }  

    }
}
