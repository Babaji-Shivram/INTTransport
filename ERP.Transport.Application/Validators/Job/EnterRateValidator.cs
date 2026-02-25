using ERP.Transport.Application.DTOs.Job;
using FluentValidation;

namespace ERP.Transport.Application.Validators.Job;

public class EnterRateValidator : AbstractValidator<EnterRateDto>
{
    public EnterRateValidator()
    {
        RuleFor(x => x.FreightRate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DetentionCharges).GreaterThanOrEqualTo(0);
        RuleFor(x => x.VaraiCharges).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EmptyContainerReturn).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TollCharges).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OtherCharges).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(3);
    }
}
