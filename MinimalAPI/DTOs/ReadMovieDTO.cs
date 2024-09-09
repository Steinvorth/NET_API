namespace MinimalAPI.DTOs
{
    public class ReadMovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster { get; set; }
        public List<ReadGenreDTO> Genres = new List<ReadGenreDTO>();
        public List<MovieActorDTO> Actors = new List<MovieActorDTO>();

    }
}
