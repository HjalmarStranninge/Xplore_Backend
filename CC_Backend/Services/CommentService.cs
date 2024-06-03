using System.ComponentModel;
using CC_Backend.Models;
using CC_Backend.Models.DTOs;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Repositories.CommentRepo;
using CC_Backend.Repositories.StampsRepo;

namespace CC_Backend.Services
{
    public class CommentService :ICommentService
    {
        private readonly IStampsRepo _stampsRepo;
        private readonly ICommentRepo _commentRepo;

        public CommentService(IStampsRepo stampsRepo, ICommentRepo commentRepo)
        {
            _stampsRepo = stampsRepo;
            _commentRepo = commentRepo;
        }

        public async Task<bool> AddCommentAsync(CommentCreateDTO dto, string userID)
        {
            var stampCollected = await _stampsRepo.GetStampCollectedAsync(dto.StampCollectedId);
            if(stampCollected == null)
            {
                throw new ArgumentException("The specified stamp collected does not exist.");
            }

            var now = DateTime.UtcNow; // Use to avoid timezone issues
            var createdAt = new DateTime(now.Year, now.Month, now.Minute);

            var comment = new Comment
            {
                StampCollectedId = dto.StampCollectedId,
                Content = dto.Content,
                StampCollected = stampCollected,
                UserId = userID,
                CreatedAt = createdAt
            };

            await _commentRepo.AddCommentAsync(comment);
            return true;
        }

        public async Task<List<CommentViewModel>> ListCommentsFromStampCollected(int stampCollectedId)
        {
            ICollection<Comment> comments = await _commentRepo.GetCommentsFromStampCollectedAsync(stampCollectedId);
            
            var commentViewModels = new List<CommentViewModel>();
            foreach (var comment in comments)
            {
                var commentViewModel = new CommentViewModel
                {
                    CommenterDisplayName = comment.User.DisplayName,
                    CommenterProfilePic = comment.User.ProfilePicture,
                    CommentContent = comment.Content,
                    PostedAt = comment.CreatedAt,
                };

                commentViewModels.Add(commentViewModel);
            }
            return commentViewModels;
        }
    }
}
