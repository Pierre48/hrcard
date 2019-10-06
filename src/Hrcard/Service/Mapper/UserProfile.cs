using AutoMapper;
using MyCompany.Models;
using MyCompany.Service.Dto;

namespace MyCompany.Service.Mapper {
    public class UserProfile : Profile {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
