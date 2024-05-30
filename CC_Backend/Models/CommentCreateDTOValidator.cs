using CC_Backend.Models.DTOs;
using FluentValidation;

namespace CC_Backend.Models
{
    public class CommentCreateDTOValidator : AbstractValidator<CommentCreateDTO>
    {
        public CommentCreateDTOValidator()
        {
            RuleFor(x => x.StampCollectedId).GreaterThan(0).WithMessage("StampCollectedId must be greater than zero.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content cannot be empty.");
        }
    }
}
