using AutoMapper;
using AuthService.Entities;
using AuthService.Models;

namespace UserMappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();
        }
    }
}