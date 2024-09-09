using Microsoft.EntityFrameworkCore;
using MinimalAPI.DTOs;
using MinimalAPI.Entities;
using MinimalAPI.Utilities;

namespace MinimalAPI.Repository
{
    public class RepositoryMovie : IRepositoryMovie
    {
        private readonly ApplicationDBContext context;
        private readonly HttpContext httpContext;

        public RepositoryMovie(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Movie>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Movies.AsQueryable();
            await httpContext.InsertPaginationParametersInResponse(queryable);

            return await queryable
                .OrderBy(x => x.Title)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Movie> GetById(int id)
        {
            return await context.Movies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Movie movie)
        {
            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;
        }

        public async Task Update(Movie movie)
        {
            context.Update(movie);
            await context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await context.Movies.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Movies.AnyAsync(x => x.Id == id);
        }
    }
}
