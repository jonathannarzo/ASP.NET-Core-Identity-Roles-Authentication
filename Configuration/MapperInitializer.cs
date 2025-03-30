namespace api.Configuration;

using api.Models;
using AutoMapper;

public class MapperInitializer : Profile
{
    public MapperInitializer()
    {
        CreateMap<ApiUser, UserDTO>().ReverseMap();
        CreateMap<ApiUser, UpdateUserDTO>().ReverseMap();
        CreateMap<Roles, RoleDTO>().ReverseMap();
    }
}