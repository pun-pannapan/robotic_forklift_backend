namespace Robotic.Forklift.Domain.Entities
{
    public class ForkliftCommand
    {
        public int Id { get; set; }
        public int ForkliftId { get; set; }
        public Forklift? Forklift { get; set; }
        public int IssuedByUserId { get; set; }
        public User? IssuedBy { get; set; }
        public string Command { get; set; } = null!;
        public string ParsedActionsJson { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}