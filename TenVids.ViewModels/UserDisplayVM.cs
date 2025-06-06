﻿
namespace TenVids.ViewModels
{
    public  class UserDisplayVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public bool IsLocked { get; set; }
        public DateTime LockEndsIn { get; set; }
        public IEnumerable<string> AssignedRoles { get; set; }
    }
}
