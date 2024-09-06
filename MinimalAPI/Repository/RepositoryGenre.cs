using Microsoft.EntityFrameworkCore;
using MinimalAPI.Entities;

namespace MinimalAPI.Repository
{
    public class RepositoryGenre : IRepositoryGenre
    {
        private readonly ApplicationDBContext _dbContext;
        public RepositoryGenre(ApplicationDBContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<int> Create(Genre genre)
        {
            _dbContext.Add(genre);  
            await _dbContext.SaveChangesAsync();
            return genre.Id;
        }

        public async Task Delete(int id)
        {
            await _dbContext.Genres.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _dbContext.Genres.AnyAsync(x => x.Id == id);
        }

        public async Task<List<Genre>> GetAll()
        {
            return await _dbContext.Genres.OrderBy(x => x.Name).ToListAsync(); //orders by name
        }

        public async Task<Genre?> GetById(int id)
        {
            return await _dbContext.Genres.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(Genre genre)
        {
            _dbContext.Update(genre);
            await _dbContext.SaveChangesAsync();
        }
    }
}
