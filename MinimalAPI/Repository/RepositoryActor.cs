using Microsoft.EntityFrameworkCore;
using MinimalAPI.DTOs;
using MinimalAPI.Entities;
using MinimalAPI.Utilities;

namespace MinimalAPI.Repository
{
    public class RepositoryActor : IRepositoryActor
    {
        private readonly ApplicationDBContext _context;
        private readonly HttpContext _httpContext;
        public RepositoryActor(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Actor>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = _context.Actors.AsQueryable();
            await _httpContext.InsertPaginationParametersInResponse(queryable);
            return await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
        }

        public async Task<Actor?> GetById(int id)
        {
            return await _context.Actors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Actor>> GetByName(string name)
        {
            return await _context.Actors.Where(x => x.Name.Contains(name)).OrderBy(x => x.Name).ToListAsync();
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

        public async Task<List<int>> ActorExist(List<int> ids)
        {
            return await _context.Actors.Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToListAsync();
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
