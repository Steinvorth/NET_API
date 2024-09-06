using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
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

        static async Task<Ok<List<Genre>>> GetGenres(IRepositoryGenre repository)
        {
            var genres = await repository.GetAll();
            return TypedResults.Ok(genres);
        }

        static async Task<Results<Ok<Genre>, NotFound>> GetGenreById(IRepositoryGenre repository, int id)
        {
            var genre = await repository.GetById(id);

            if (genre == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(genre);
        }

        static async Task<Created<Genre>> CreateGenre(Genre genre, IRepositoryGenre repository, IOutputCacheStore outputCache)
        {
            var id = await repository.Create(genre);

            await outputCache.EvictByTagAsync("genre-get", default); //cleans the cache of the method with the tag.

            return TypedResults.Created($"/genre/{id}", genre);
        }

        static async Task<Results<NoContent, NotFound>> UpdateGenre(Genre genre, IRepositoryGenre repository, IOutputCacheStore outputCache, int id)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }

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
