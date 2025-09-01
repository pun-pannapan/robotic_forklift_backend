using Robotic.Forklift.Application.Dtos;

namespace Robotic.Forklift.Application.Interfaces
{
    public interface ICommandParser
    {
        List<ParsedActionDto> Parse(string command);
    }
}
