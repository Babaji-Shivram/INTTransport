using ERP.Transport.Application.DTOs.Job;
using FluentValidation;

namespace ERP.Transport.Application.Validators.Job;

public class AddMovementValidator : AbstractValidator<AddMovementDto>
{
    public AddMovementValidator()
    {
        RuleFor(x => x.Milestone).IsInEnum();
        RuleFor(x => x.Timestamp).NotEmpty();
    }
}
