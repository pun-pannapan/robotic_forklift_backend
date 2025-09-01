namespace Robotic.Forklift.Domain.Entities
{
    public class Forklift
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ModelNumber { get; set; } = null!;
        public DateTime ManufacturingDate { get; set; }
        public ICollection<ForkliftCommand> Commands { get; set; } = new List<ForkliftCommand>();
    }
}