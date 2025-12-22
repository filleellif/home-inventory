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
    }
}
