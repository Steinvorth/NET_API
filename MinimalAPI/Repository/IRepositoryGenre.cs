using MinimalAPI.Entities;

namespace MinimalAPI.Repository
{
    public interface IRepositoryGenre
    {
        Task<int> Create(Genre genre);
    }
}
