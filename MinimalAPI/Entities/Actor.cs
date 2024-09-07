using Microsoft.EntityFrameworkCore;

namespace MinimalAPI.Entities
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        [Unicode(true)]
        public string? Picture { get; set; } 
    }
}
