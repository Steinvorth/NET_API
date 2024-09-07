using AutoMapper;
using MinimalAPI.DTOs;
using MinimalAPI.Entities;

namespace MinimalAPI.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //Genre
            CreateMap<CreateGrenreDTO, Genre>();
            CreateMap<Genre, ReadGenreDTO>();

            //Actor
            CreateMap<CreateActorDTO, Actor>().ForMember(x => x.Picture, options => options.Ignore()); //ignores the picture property
            CreateMap<Actor, ReadActorDTO>();
        }
    }
}
