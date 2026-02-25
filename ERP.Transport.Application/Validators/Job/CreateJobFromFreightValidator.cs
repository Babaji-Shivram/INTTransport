using ERP.Transport.Application.DTOs.Job;
using FluentValidation;

namespace ERP.Transport.Application.Validators.Job;

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
