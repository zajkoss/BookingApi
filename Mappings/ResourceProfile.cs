using AutoMapper;
using BookingApi.Commands;
using BookingApi.DTOs;
using BookingApi.Models;

namespace BookingApi.Mappings;

public class ResourceProfile : Profile
{
    public ResourceProfile()
    {
        CreateMap<Resource, ResourceDto>()
            .ConstructUsing(src => new ResourceDto(src.Id, src.Name, src.Description, src.Capacity, src.IsActive));
        CreateMap<CreateResourceCommand, Resource>();
        CreateMap<UpdateResourceCommand, Resource>();
        
    }
}