using BookingApi.Commands;
using BookingApi.Repository;
using FluentValidation;

namespace BookingApi.Validators;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{

    private readonly IResourceRepository _resourceRepository;
    public CreateBookingCommandValidator(IResourceRepository resourceRepository)
    {
        _resourceRepository = resourceRepository;
        RuleFor(x => x.ResourceId).NotEmpty()
            .MustAsync(async (id, token) => await _resourceRepository.GetByIdAsync(id) is not null)
            .WithMessage("Resource does not exist");    
        RuleFor(x => x.StartDate).NotEmpty()
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before end date");
        RuleFor(x => x.BookingBy).NotEmpty();
    }
}