using MinimalAPI.DTOs;
using MinimalAPI.Entities;

namespace MinimalAPI.Repository
{
    public interface IRepositoryActor
    {
        Task<int> Create(Actor actor);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<List<Actor>> GetAll(PaginationDTO paginationDTO);
        Task<Actor?> GetById(int id);
        Task<List<Actor>> GetByName(string name);
        Task Update(Actor actor);
    }
}