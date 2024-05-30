using CC_Backend.Models.DTOs;
using FluentValidation;

namespace CC_Backend.Models
{
    public class ResetPasswordDTOValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token is required.");
            RuleFor(x => x.newPassword).NotEmpty().WithMessage("New password is required.");
        }
    }
}
