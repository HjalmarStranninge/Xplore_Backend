namespace CC_Backend.Models.Viewmodels
{
    public class CommentViewModel
    {
        public string CommenterDisplayName {  get; set; }
        public byte[]? CommenterProfilePic { get; set; }
        public string CommentContent { get; set; }
        public DateTime? PostedAt { get; set; }
    }
}
