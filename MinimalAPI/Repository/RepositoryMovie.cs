using AutoMapper;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.DTOs;
using MinimalAPI.Entities;
using MinimalAPI.Utilities;

namespace MinimalAPI.Repository
{
    public class RepositoryMovie : IRepositoryMovie
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public RepositoryMovie(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
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

        public async Task<Movie?> GetById(int id)
        {
            return await context.Movies
                .Include(x => x.MovieGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MovieActors).ThenInclude(x => x.Actor)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task AsignGenre(int id, List<int> genreIds)
        {
            var movie = await context.Movies.Include(x => x.MovieGenres).FirstOrDefaultAsync(p => p.Id == id);

            if (movie == null)
            {
                throw new ApplicationException($"The movie with id {id} does not exist");
            }

            var genreMovies = genreIds.Select(genreId => new MovieGenre() { GenreId = genreId });

            movie.MovieGenres = mapper.Map(genreMovies, movie.MovieGenres);

            await context.SaveChangesAsync();
        }

        public async Task AsignActors(int id, List<MovieActors> actors)
        {
            for(int i = 1; i <= actors.Count; i++)
            {
                actors[i-1].Order = i;
            }

            var movie = await context.Movies.Include(x => x.MovieActors).FirstOrDefaultAsync(p => p.Id == id);

            if(movie == null)
            {
                throw new ApplicationException($"The movie with id {id} does not exist");
            }

            movie.MovieActors = mapper.Map(actors, movie.MovieActors);

            await context.SaveChangesAsync();
        }
    }
}
