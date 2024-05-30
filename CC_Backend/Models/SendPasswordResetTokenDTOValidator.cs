using CC_Backend.Models.DTOs;
using FluentValidation;

namespace CC_Backend.Models
{
    public class SendPasswordResetTokenDTOValidator : AbstractValidator<SendPasswordResetTokenDto>
    {
        public SendPasswordResetTokenDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
        }
    }
}
