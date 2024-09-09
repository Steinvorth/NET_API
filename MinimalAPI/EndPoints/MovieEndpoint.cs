using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPI.DTOs;
using MinimalAPI.Entities;
using MinimalAPI.Repository;
using MinimalAPI.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MinimalAPI.EndPoints
{
    public static class MovieEndpoint
    {
        private static readonly string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapGet("/", Get).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("movies-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete).DisableAntiforgery();
            group.MapPost("/{id:int}/genre", AsignGenre);
            group.MapPost("/{id:int}/actor", AsignActors).DisableAntiforgery();

            return group;
        }

        static async Task<Ok<List<ReadMovieDTO>>> Get(IRepositoryMovie repository, IMapper mapper, int page = 1, int recordPerPage = 10)
        {
            var paginationDTO = new PaginationDTO { Page = page, RecordsPerPage = recordPerPage };
            var movies = await repository.GetAll(paginationDTO);
            var moviesDTO = mapper.Map<List<ReadMovieDTO>>(movies);
            return TypedResults.Ok(moviesDTO);
        }

        static async Task<Results<Ok<ReadMovieDTO>, NotFound>> GetById(int id, IRepositoryMovie repository, IMapper mapper)
        {
            var movie = await repository.GetById(id);
            if (movie is null)
            {
                return TypedResults.NotFound();
            }
            var movieDTO = mapper.Map<ReadMovieDTO>(movie);
            return TypedResults.Ok(movieDTO);
        }

        static async Task<Created<ReadMovieDTO>> Create([FromForm] CreateMovieDTO createMovieDTO,
            IRepositoryMovie repository, I_FileStore fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var movie = mapper.Map<Movie>(createMovieDTO);

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Save(createMovieDTO.Poster, container);
                movie.Poster = url;
            }

            var id = await repository.Create(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);

            var readMovieDTO = mapper.Map<ReadMovieDTO>(movie);
            return TypedResults.Created($"/movies/{id}", readMovieDTO);

        }

        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateMovieDTO createMovieDTO, IRepositoryMovie repository,
            I_FileStore fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var dbMovie = await repository.GetById(id);

            if(dbMovie is null)
            {
                return TypedResults.NotFound();
            }

            var movieUpdate = mapper.Map(createMovieDTO, dbMovie);
            movieUpdate.Id = id;
            movieUpdate.Poster = dbMovie.Poster;

            if(createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Edit(movieUpdate.Poster, container, createMovieDTO.Poster);
                movieUpdate.Poster = url;         
            }

            await repository.Update(movieUpdate);
            await outputCacheStore.EvictByTagAsync("movies-get", default);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IRepositoryMovie repository, IOutputCacheStore outputCacheStore,
            I_FileStore fileStorage)
        {
            var dbMovie = await repository.GetById(id);

            if (dbMovie is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await fileStorage.Delete(dbMovie.Poster, container);
            await outputCacheStore.EvictByTagAsync("movies-get", default);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignGenre(int id, List<int> genreIds, IRepositoryMovie repositoryMovie, IRepositoryGenre repositoryGenre)
        {
            if(!await repositoryMovie.Exists(id))
            {
                return TypedResults.NotFound();
            }

            var existingGenres = await repositoryGenre.GenreExists(genreIds);

            if (existingGenres.Count != 0)
            {
                existingGenres = await repositoryGenre.GenreExists(genreIds);
            }

            if (existingGenres.Count != genreIds.Count)
            {
                var nonExistenGenres = genreIds.Except(existingGenres);

                return TypedResults.BadRequest($"Genres with ids {string.Join(",", nonExistenGenres)} do not exist");
            }

            await repositoryMovie.AsignGenre(id, genreIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AsignActors(int id, List<AsignMovieActorDTO> movieActorsDTO,
            IRepositoryMovie repositoryMovie, IRepositoryActor repositoryActor, IMapper mapper)
        {
            if(!await repositoryMovie.Exists(id))
            {
                return TypedResults.NotFound();
            }

            var existingActors = new List<int>();
            var actorIds = movieActorsDTO.Select(x => x.ActorId).ToList();

            if(actorIds.Count != 0)
            {
                existingActors = await repositoryActor.ActorExist(actorIds);
            }

            if (existingActors.Count != movieActorsDTO.Count)
            {
                var nonExistentActors = actorIds.Except(existingActors);
                return TypedResults.BadRequest($"Actors with ids {string.Join(",", nonExistentActors)} do not exist");
            }

            var actors = mapper.Map<List<MovieActors>>(movieActorsDTO);
            await repositoryMovie.AsignActors(id, actors);

            return TypedResults.NoContent();
        }
    }
}
