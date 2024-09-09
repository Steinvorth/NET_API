using Microsoft.EntityFrameworkCore;

namespace MinimalAPI.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }

        [Unicode]
        public string? Poster { get; set; }
    }
}
