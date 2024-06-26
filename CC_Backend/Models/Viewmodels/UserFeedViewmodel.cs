﻿namespace CC_Backend.Models.Viewmodels
{
    public class UserFeedViewmodel
    {
        public string? DisplayName { get; set; }
        public int? StampCollectedId { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string? Category { get; set; }
        public byte[]? StampIcon { get; set; }
        public string? StampName { get; set; }
        public DateTime? DateCollected { get; set; }
        public ICollection<CommentViewModel> Comments { get; set; }
        public int LikeCount { get; set; }
    }
}
