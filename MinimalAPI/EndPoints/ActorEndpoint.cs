using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPI.DTOs;
using MinimalAPI.Entities;
using MinimalAPI.Repository;
using MinimalAPI.Services;
using System.Text.RegularExpressions;

namespace MinimalAPI.EndPoints
{
    public static class ActorEndpoint
    {
        private static readonly string container = "actors";
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("actors-get"));
            group.MapGet("getByName/{name}", GetByName);
            group.MapGet("/{id:int}", GetById);
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete).DisableAntiforgery();
            return group;
        }

        static async Task<Ok<List<ReadActorDTO>>> GetAll(IRepositoryActor repository, IMapper mapper, int page =1, int recordsPerPage = 10)
        {
            var paginationDTO = new PaginationDTO
            {
                Page = page,
                RecordsPerPage = recordsPerPage
            };
            var actors = await repository.GetAll(paginationDTO);
            var actorsDTO = mapper.Map<List<ReadActorDTO>>(actors);
            return TypedResults.Ok(actorsDTO);
        }

        static async Task<Results<Ok<ReadActorDTO>,NotFound>> GetById(int id, IRepositoryActor repository, IMapper mapper)
        {
            var actor = await repository.GetById(id);
            if (actor is null)
            {
                return TypedResults.NotFound();
            }
            var actorDTO = mapper.Map<ReadActorDTO>(actor);
            return TypedResults.Ok(actorDTO);
        }

        static async Task<Ok<List<ReadActorDTO>>> GetByName(string name, IRepositoryActor repository, IMapper mapper)
        {
            var actors = await repository.GetByName(name);
            var actorsDTO = mapper.Map<List<ReadActorDTO>>(actors);
            return TypedResults.Ok(actorsDTO);
        }

        public static async Task<Created<ReadActorDTO>> Create([FromForm] CreateActorDTO createActorDTO, 
            IRepositoryActor repository, IOutputCacheStore outputCache, IMapper mapper, I_FileStore storeFile)
        {
            var actor = mapper.Map<Actor>(createActorDTO);

            if(createActorDTO.Picture is not null)
            {
                var picture_url = await storeFile.Save(createActorDTO.Picture, container);
                actor.Picture = picture_url;
            }

            var id = await repository.Create(actor);
            await outputCache.EvictByTagAsync("actors-get", default);
            var actorDTO = mapper.Map<ReadActorDTO>(actor);
            return TypedResults.Created($"actors/{id}", actorDTO);
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateActorDTO createActorDTO,
        IRepositoryActor repository, I_FileStore fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var dbActor = await repository.GetById(id);

            if (dbActor is null)
            {
                return TypedResults.NotFound();
            }

            var updatedActor = mapper.Map<Actor>(createActorDTO);
            updatedActor.Id = id;
            updatedActor.Picture = dbActor.Picture;

            if (createActorDTO.Picture is not null)
            {
                var picture_url = await fileStorage.Edit(updatedActor.Picture, container, createActorDTO.Picture);
                updatedActor.Picture = picture_url;

                updatedActor.Picture = picture_url;
            }

            await repository.Update(updatedActor);
            await outputCacheStore.EvictByTagAsync("actors-get", default);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IRepositoryActor repository, 
            IOutputCacheStore outputCacheStore, I_FileStore fileStorage)
        {
            var dbActor = await repository.GetById(id);
            
            if (dbActor is null)
            {
                return TypedResults.NoContent();
            }

            await repository.Delete(id);
            await fileStorage.Delete(dbActor.Picture, container);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }
    }    
}
