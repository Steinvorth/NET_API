using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPI.DTOs;
using MinimalAPI.Entities;
using MinimalAPI.Repository;
using System.Runtime.CompilerServices;

namespace MinimalAPI.EndPoints
{
    public static class GenreEndpoint
    {        
        public static RouteGroupBuilder MapGenre(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenres).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("genre-get")); //cache the response for 15 seconds

            group.MapGet("/{id:int}", GetGenreById);

            //post request to create a genre
            group.MapPost("/", CreateGenre);

            group.MapPut("/{id:int}", UpdateGenre);

            group.MapDelete("/{id:int}", DeleteGenre);

            return group;
        }

        static async Task<Ok<List<ReadGenreDTO>>> GetGenres(IRepositoryGenre repository, IMapper mapper)
        {
            var genres = await repository.GetAll();

            var genresDTO = mapper.Map<List<ReadGenreDTO>>(genres);

            return TypedResults.Ok(genresDTO);
        }

        static async Task<Results<Ok<ReadGenreDTO>, NotFound>> GetGenreById(IRepositoryGenre repository, int id, IMapper mapper)
        {
            var genre = await repository.GetById(id);

            if (genre == null)
            {
                return TypedResults.NotFound();
            }

            var genreDTO = mapper.Map<ReadGenreDTO>(genre);

            return TypedResults.Ok(genreDTO);
        }

        static async Task<Created<ReadGenreDTO>> CreateGenre(CreateGrenreDTO createGenreDTO, IRepositoryGenre repository, IOutputCacheStore outputCache, IMapper mapper)
        {
            var genre = mapper.Map<Genre>(createGenreDTO);

            var id = await repository.Create(genre);

            await outputCache.EvictByTagAsync("genre-get", default); //cleans the cache of the method with the tag.

            var genreDTO = mapper.Map<ReadGenreDTO>(genre);

            return TypedResults.Created($"/genre/{id}", genreDTO);
        }

        static async Task<Results<NoContent, NotFound>> UpdateGenre(CreateGrenreDTO createGrenreDTO, IRepositoryGenre repository, IOutputCacheStore outputCache, int id, IMapper mapper)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }

            var genre = mapper.Map<Genre>(createGrenreDTO);
            genre.Id = id;

            await repository.Update(genre);
            await outputCache.EvictByTagAsync("genre-get", default); //cleans the cache of the method with the tag.
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> DeleteGenre(IRepositoryGenre repository, IOutputCacheStore outputCache, int id)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await outputCache.EvictByTagAsync("genre-get", default); //cleans the cache of the method with the tag.
            return TypedResults.NoContent();
        }

    }
}
