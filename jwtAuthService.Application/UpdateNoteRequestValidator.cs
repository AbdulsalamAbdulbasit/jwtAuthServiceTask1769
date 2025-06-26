using FluentValidation;

namespace jwtAuthService.Application.Validators
{
    public class UpdateNoteRequestValidator : AbstractValidator<DTOs.UpdateNoteRequest>
    {
        public UpdateNoteRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
        }
    }
}
