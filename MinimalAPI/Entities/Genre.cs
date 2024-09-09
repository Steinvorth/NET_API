using System.ComponentModel.DataAnnotations;

namespace MinimalAPI.Entities
{
    public class Genre
    {
        public int Id { get; set; }

        //[StringLength(50)] these tags will also limit and modify the database schema
        public string Name { get; set; } = null!;

        public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    }
}
