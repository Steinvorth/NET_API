using Microsoft.EntityFrameworkCore;
using MinimalAPI.Entities;

namespace MinimalAPI.Repository
{
    public class RepositoryActor : IRepositoryActor
    {
        private readonly ApplicationDBContext _context;
        public RepositoryActor(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Actor>> GetAll()
        {
            return await _context.Actors.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<Actor?> GetById(int id)
        {
            return await _context.Actors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Actor actor)
        {
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Actors.AnyAsync(x => x.Id == id);
        }

        public async Task Update(Actor actor)
        {
            _context.Update(actor);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Actors.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
    }
}
