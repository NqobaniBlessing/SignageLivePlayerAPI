using AutoMapper;
using SignageLivePlayerAPI.Models;
using SignageLivePlayerAPI.Models.DTOs;
using System;

namespace SignageLivePlayerAPI.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Player, PlayerCreateDTO>().ReverseMap();
            CreateMap<Player, PlayerUpdateDTO>().ReverseMap();
            CreateMap<Player, PlayerDTO>().ReverseMap();
        }
    }
}
