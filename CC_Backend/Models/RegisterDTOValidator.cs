using CC_Backend.Models.DTOs;
using FluentValidation;

namespace CC_Backend.Models

{
    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(dto => dto.Email).NotEmpty()
                .Must(BeValidEmail).WithMessage("Invalid email domain. The email address must end with '.se' or '.com'.");
        }

        private bool BeValidEmail(string email)
        {
            return email.EndsWith(".se", StringComparison.OrdinalIgnoreCase) || 
                email.EndsWith(".com", StringComparison.OrdinalIgnoreCase);
        }
    }
}
