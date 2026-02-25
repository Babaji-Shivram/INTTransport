using ERP.Transport.Application.DTOs.Job;
using FluentValidation;

namespace ERP.Transport.Application.Validators.Job;

public class AssignVehicleValidator : AbstractValidator<AssignVehicleDto>
{
    public AssignVehicleValidator()
    {
        RuleFor(x => x.TransporterId).NotEmpty().WithMessage("Transporter is required");
        RuleFor(x => x.VehicleNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.VehicleType).IsInEnum();
    }
}
