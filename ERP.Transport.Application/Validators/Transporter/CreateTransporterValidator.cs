using ERP.Transport.Application.DTOs.Transporter;
using FluentValidation;

namespace ERP.Transport.Application.Validators.Transporter;

public class CreateTransporterValidator : AbstractValidator<CreateTransporterDto>
{
    public CreateTransporterValidator()
    {
        RuleFor(x => x.TransporterName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Phone).MaximumLength(20);
    }
}
