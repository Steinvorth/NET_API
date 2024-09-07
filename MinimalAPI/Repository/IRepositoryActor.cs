using MinimalAPI.Entities;

namespace MinimalAPI.Repository
{
    public interface IRepositoryActor
    {
        Task<int> Create(Actor actor);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<List<Actor>> GetAll();
        Task<Actor?> GetById(int id);
        Task Update(Actor actor);
    }
}