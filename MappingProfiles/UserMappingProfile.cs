using AutoMapper;
using AuthService.Entities;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;

namespace UserMappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDto, IdentityUser>();
            CreateMap<IdentityUser, UserDto>();
        }
    }
}