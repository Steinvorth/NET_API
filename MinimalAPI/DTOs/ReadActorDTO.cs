namespace MinimalAPI.DTOs
{
    public class ReadActorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string? Picture { get; set; }
    }
}
