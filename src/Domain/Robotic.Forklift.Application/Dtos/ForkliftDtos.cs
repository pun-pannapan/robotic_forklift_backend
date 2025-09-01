namespace Robotic.Forklift.Application.Dtos
{    
    public record ForkliftDto(int Id, string Name, string ModelNumber, DateTime ManufacturingDate);
    public record ImportResultDto(int Inserted, int Skipped, List<string> Errors);
}