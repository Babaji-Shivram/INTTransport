using ERP.Transport.Application.DTOs.Job;
using FluentValidation;

namespace ERP.Transport.Application.Validators.Job;

public class RecordDeliveryValidator : AbstractValidator<RecordDeliveryDto>
{
    public RecordDeliveryValidator()
    {
        RuleFor(x => x.DeliveryDate).NotEmpty();
        RuleFor(x => x.ReceivedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DeliveryStatus).IsInEnum();
    }
}
