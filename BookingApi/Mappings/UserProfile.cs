using AutoMapper;
using BookingApi.DTOs;
using BookingApi.Models;

namespace BookingApi.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ConstructUsing(src => new UserDto(src.Id, src.Email, src.FirstName, src.LastName, src.CreatedAt, src.IsActive));
    }
}