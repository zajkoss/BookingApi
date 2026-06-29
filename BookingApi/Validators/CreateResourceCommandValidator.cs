using BookingApi.Commands;
using FluentValidation;

namespace BookingApi.Validators;

public class CreateResourceCommandValidator : AbstractValidator<CreateResourceCommand>
{
    public CreateResourceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Capacity)
            .GreaterThan(0);

    }
}