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

            //Movie
            CreateMap<CreateMovieDTO, Movie>().ForMember(x => x.Poster, options => options.Ignore());
            CreateMap<Movie, ReadMovieDTO>()
                .ForMember(x => x.Genres, entity => entity
                    .MapFrom(p => p.MovieGenres.Select(x => new ReadGenreDTO { Id = x.GenreId, Name =  x.Genre.Name })))
                .ForMember(x => x.Actors, entity => entity
                    .MapFrom(x => x.MovieActors.Select(x => new MovieActorDTO { Id = x.ActorId, Name = x.Actor.Name, Character = x.Character })));

            //MovieActors
            CreateMap<AsignMovieActorDTO, MovieActors>();
            
        }
    }
}
