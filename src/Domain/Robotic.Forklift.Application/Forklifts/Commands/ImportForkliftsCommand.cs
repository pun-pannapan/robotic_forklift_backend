using MediatR;
using Microsoft.EntityFrameworkCore;
using Robotic.Forklift.Application.Dtos;
using Robotic.Forklift.Application.Interfaces;
using Robotic.Forklift.Application.Validations;
using System.Globalization;
using System.Text.Json;

namespace Robotic.Forklift.Application.Forklifts.Commands
{
    public record ImportForkliftsCommand(string FileName, string TextContent) : IRequest<ImportResultDto>;

    public class ImportForkliftsHandler : IRequestHandler<ImportForkliftsCommand, ImportResultDto>
    {
        private readonly IAppDbContext _db;
        private readonly ImportRowValidator _validator = new();
        public ImportForkliftsHandler(IAppDbContext db) 
        { 
            _db = db; 
        }

        public async Task<ImportResultDto> Handle(ImportForkliftsCommand request, CancellationToken ct)
        {
            var rows = new List<ImportRow>();
            var ext = Path.GetExtension(request.FileName).ToLowerInvariant();
            if (ext == ".json")
            {
                var parsed = JsonSerializer.Deserialize<List<ImportRow>>(request.TextContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (parsed != null) rows.AddRange(parsed);
            }
            else if (ext == ".csv")
            {
                using var reader = new StringReader(request.TextContent);
                var header = reader.ReadLine();
                while (true)
                {
                    var line = reader.ReadLine(); if (line == null)
                    {
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var parts = line.Split(',');

                    if (parts.Length < 3)
                    {
                        continue;
                    }

                    if (!DateTime.TryParse(parts[2], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt))
                    {
                        continue;
                    }

                    rows.Add(new ImportRow { Name = parts[0].Trim(), ModelNumber = parts[1].Trim(), ManufacturingDate = dt });
                }
            }
            else
            {
                throw new InvalidOperationException("Unsupported file type. Use .csv or .json");
            }

            var errors = new List<string>();
            var toInsert = new List<Forklift.Domain.Entities.Forklift>();
            foreach (var row in rows)
            {
                var validate = await _validator.ValidateAsync(row, ct);
                if (!validate.IsValid)
                { 
                    errors.AddRange(validate.Errors.Select(e => $"{row.Name}: {e.ErrorMessage}"));
                    continue;
                }

                bool exists = await _db.Forklifts.AnyAsync(f => f.Name == row.Name && f.ModelNumber == row.ModelNumber, ct);
                if (exists)
                {
                    continue;
                }

                toInsert.Add(new Forklift.Domain.Entities.Forklift { Name = row.Name, ModelNumber = row.ModelNumber, ManufacturingDate = row.ManufacturingDate });
            }

            _db.Forklifts.AddRange(toInsert);
            var inserted = await _db.SaveChangesAsync(ct);
            return new ImportResultDto(inserted, rows.Count - inserted, errors);
        }
    }
}
