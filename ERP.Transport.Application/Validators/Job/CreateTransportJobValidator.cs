using ERP.Transport.Application.DTOs.Job;
using FluentValidation;

namespace ERP.Transport.Application.Validators.Job;

public class CreateTransportJobValidator : AbstractValidator<CreateTransportJobDto>
{
    public CreateTransportJobValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer is required");
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PickupAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DropAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CargoType).IsInEnum();
        RuleFor(x => x.GrossWeightKg).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NumberOfPackages).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Container20Count).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Container40Count).GreaterThanOrEqualTo(0);
        RuleFor(x => x.VehicleTypeRequired).IsInEnum();
        RuleFor(x => x.DeliveryType).IsInEnum();
        RuleFor(x => x.Priority).IsInEnum();
    }
}
