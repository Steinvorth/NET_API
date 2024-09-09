using Microsoft.EntityFrameworkCore;

namespace MinimalAPI.DTOs
{
    public class CreateMovieDTO
    {
        public string Title { get; set; } = null!;
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IFormFile? Poster { get; set; }
    }
}
