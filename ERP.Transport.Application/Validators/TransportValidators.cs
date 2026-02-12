using ERP.Transport.Application.DTOs;
using FluentValidation;

namespace ERP.Transport.Application.Validators;

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

public class AssignVehicleValidator : AbstractValidator<AssignVehicleDto>
{
    public AssignVehicleValidator()
    {
        RuleFor(x => x.TransporterId).NotEmpty().WithMessage("Transporter is required");
        RuleFor(x => x.VehicleNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.VehicleType).IsInEnum();
    }
}

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

public class RecordDeliveryValidator : AbstractValidator<RecordDeliveryDto>
{
    public RecordDeliveryValidator()
    {
        RuleFor(x => x.DeliveryDate).NotEmpty();
        RuleFor(x => x.ReceivedBy).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DeliveryStatus).IsInEnum();
    }
}

public class AddMovementValidator : AbstractValidator<AddMovementDto>
{
    public AddMovementValidator()
    {
        RuleFor(x => x.Milestone).IsInEnum();
        RuleFor(x => x.Timestamp).NotEmpty();
    }
}

public class CreateTransporterValidator : AbstractValidator<CreateTransporterDto>
{
    public CreateTransporterValidator()
    {
        RuleFor(x => x.TransporterName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Phone).MaximumLength(20);
    }
}

public class CreateJobFromEnquiryValidator : AbstractValidator<CreateJobFromEnquiryDto>
{
    public CreateJobFromEnquiryValidator()
    {
        RuleFor(x => x.EnquiryId).NotEmpty();
        RuleFor(x => x.EnquiryReferenceNumber).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.PickupAddress).NotEmpty();
        RuleFor(x => x.DropAddress).NotEmpty();
        RuleFor(x => x.CountryCode).NotEmpty().MaximumLength(5);
        RuleFor(x => x.BranchId).NotEmpty();
    }
}

public class CreateJobFromFreightValidator : AbstractValidator<CreateJobFromFreightDto>
{
    public CreateJobFromFreightValidator()
    {
        RuleFor(x => x.FreightJobId).NotEmpty();
        RuleFor(x => x.FreightJobReference).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.PickupAddress).NotEmpty();
        RuleFor(x => x.DropAddress).NotEmpty();
        RuleFor(x => x.CountryCode).NotEmpty().MaximumLength(5);
        RuleFor(x => x.BranchId).NotEmpty();
    }
}
