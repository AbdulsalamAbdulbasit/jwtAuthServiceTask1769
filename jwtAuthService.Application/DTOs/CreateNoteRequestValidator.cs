using FluentValidation;

namespace jwtAuthService.Application.DTOs
{
    public class CreateNoteRequestValidator : AbstractValidator<CreateNoteRequest>
    {
        public CreateNoteRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
        }
    }
}
