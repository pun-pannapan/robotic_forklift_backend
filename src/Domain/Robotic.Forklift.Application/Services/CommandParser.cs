using FluentValidation;
using Robotic.Forklift.Application.Dtos;
using Robotic.Forklift.Application.Interfaces;
using System.Text.RegularExpressions;

namespace Robotic.Forklift.Application.Services
{
    public class CommandParser : ICommandParser
    {
        private Regex ForkliftCommand = new(@"([FBLR])(\d+)");
        public List<ParsedActionDto> Parse(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) 
            {
                return new();
            }
            var normalized = command.Trim().ToUpperInvariant();
            var matches = ForkliftCommand.Matches(normalized);
            if(matches.Count == 0 || string.Concat(matches.Select(m => m.Value)) != normalized)
            {
                throw new ValidationException("Invalid command format. Example: F10R90L90B5");
            }

            var actions = new List<ParsedActionDto>();
            foreach (Match match in matches) 
            {
                var code = match.Groups[1].Value;
                var value = int.Parse(match.Groups[2].Value);
                switch (code)
                {
                    case "F": actions.Add(new("Forward", value, "metres")); break;
                    case "B": actions.Add(new("Backward", value, "metres")); break;
                    case "L": ValidateDegrees(value); actions.Add(new("Left", value, "degrees")); break;
                    case "R": ValidateDegrees(value); actions.Add(new("Right", value, "degrees")); break;
                    default: throw new ValidationException($"Unsupported code '{code}'.");
                }
            }

            return actions;
        }

        private static void ValidateDegrees(int degree)
        {
            if (degree < 0 || degree > 360) throw new ValidationException("Degrees must be between 0 and 360");
            if (degree % 90 != 0) throw new ValidationException("Degrees must be a multiple of 90");
        }
    }
}
