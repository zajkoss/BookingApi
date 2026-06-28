using AutoMapper;
using BookingApi.Commands;
using BookingApi.Models;

namespace BookingApi.Mappings;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<CreateBookingCommand, Booking>();
        CreateMap<Booking, BookingDto>()
            .ConstructUsing(src => new BookingDto(src.Id, src.ResourceId, src.StartDate, src.EndDate, src.CreatedAt, src.BookedBy));

    }
}