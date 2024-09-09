namespace MinimalAPI.DTOs
{
    public class ReadMovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster { get; set; }
    }
}
