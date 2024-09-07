using AutoMapper;
using HRMSystem.Business.DTOs;
using HRMSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMSystem.Business.MappingProfiles
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<User,UserRegisterDTO>().ReverseMap();

            CreateMap<User, UserResponseDTO>().ReverseMap();
        }
    }
}
