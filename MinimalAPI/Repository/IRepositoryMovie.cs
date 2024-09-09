using MinimalAPI.DTOs;
using MinimalAPI.Entities;

namespace MinimalAPI.Repository
{
    public interface IRepositoryMovie
    {
        Task AsignActors(int id, List<MovieActors> actors);
        Task AsignGenre(int id, List<int> genreIds);
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<List<Movie>> GetAll(PaginationDTO paginationDTO);
        Task<Movie> GetById(int id);
        Task Update(Movie movie);
    }
}