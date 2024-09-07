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
            return group;
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
    }
}
