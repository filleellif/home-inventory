using FluentValidation;

namespace HomeInventory.Application.Commands.Items.UpdateItem;

public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(200).WithMessage("Item name cannot exceed 200 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0).When(x => x.PurchasePrice.HasValue)
            .WithMessage("Purchase price cannot be negative.");

        RuleFor(x => x.CurrentValue)
            .GreaterThanOrEqualTo(0).When(x => x.CurrentValue.HasValue)
            .WithMessage("Current value cannot be negative.");

        RuleFor(x => x.PurchaseCurrency)
            .Length(3).When(x => !string.IsNullOrWhiteSpace(x.PurchaseCurrency))
            .WithMessage("Currency must be a 3-letter code (e.g., USD, EUR).");

        RuleFor(x => x.CurrentValueCurrency)
            .Length(3).When(x => !string.IsNullOrWhiteSpace(x.CurrentValueCurrency))
            .WithMessage("Currency must be a 3-letter code (e.g., USD, EUR).");

        RuleFor(x => x.PurchaseDate)
            .LessThanOrEqualTo(DateTime.UtcNow).When(x => x.PurchaseDate.HasValue)
            .WithMessage("Purchase date cannot be in the future.");

        RuleFor(x => x.GpsLatitude)
            .InclusiveBetween(-90, 90).When(x => x.GpsLatitude.HasValue)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.GpsLongitude)
            .InclusiveBetween(-180, 180).When(x => x.GpsLongitude.HasValue)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).When(x => x.Tags != null)
            .WithMessage("Each tag cannot exceed 50 characters.");
    }
}
