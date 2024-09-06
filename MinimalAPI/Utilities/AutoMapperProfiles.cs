using AutoMapper;
using MinimalAPI.DTOs;
using MinimalAPI.Entities;

namespace MinimalAPI.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CreateGrenreDTO, Genre>();
            CreateMap<Genre, ReadGenreDTO>();
        }
    }
}
