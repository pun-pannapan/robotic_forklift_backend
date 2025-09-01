using MediatR;
using Microsoft.EntityFrameworkCore;
using Robotic.Forklift.Application.Dtos;
using Robotic.Forklift.Application.Interfaces;
using System.Text.Json;

namespace Robotic.Forklift.Application.Forklifts.Commands
{
    public record GetForkliftLogsQuery(int ForkliftId, PageQuery Query) : IRequest<PagedResult<ForkliftCommandDto>>;

    public class GetForkliftLogsHandler : IRequestHandler<GetForkliftLogsQuery, PagedResult<ForkliftCommandDto>>
    {
        private readonly IAppDbContext _db;
        public GetForkliftLogsHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResult<ForkliftCommandDto>> Handle(GetForkliftLogsQuery request, CancellationToken ct)
        {
            var (page, size, sortBy, dir) = request.Query;
            var forkliftCommand = _db.ForkliftCommands
                                    .Include(l => l.IssuedBy)
                                    .Where(l => l.ForkliftId == request.ForkliftId)
                                    .AsNoTracking();

            bool desc = (dir?.ToLowerInvariant() == "desc");
            switch (sortBy?.ToLowerInvariant())
            {
                case "command":
                    forkliftCommand = desc
                        ? forkliftCommand.OrderByDescending(x => x.Command)
                        : forkliftCommand.OrderBy(x => x.Command);
                    break;
                case "forkliftid":
                    forkliftCommand = desc
                        ? forkliftCommand.OrderByDescending(x => x.ForkliftId)
                        : forkliftCommand.OrderBy(x => x.ForkliftId);
                    break;
                case "issuedbyuserid":
                    forkliftCommand = desc
                        ? forkliftCommand.OrderByDescending(x => x.IssuedByUserId)
                        : forkliftCommand.OrderBy(x => x.IssuedByUserId);
                    break;
                default:
                    forkliftCommand = desc
                        ? forkliftCommand.OrderByDescending(x => x.CreatedAt)
                        : forkliftCommand.OrderBy(x => x.CreatedAt);
                    break;
            }

            var total = await forkliftCommand.CountAsync(ct);
            var list = await forkliftCommand.Skip((page - 1) * size).Take(size).ToListAsync(ct);
            var items = list.Select(l => new ForkliftCommandDto(
                                    l.Id,
                                    l.ForkliftId,
                                    l.Command,
                                    JsonSerializer.Deserialize<List<ParsedActionDto>>(l.ParsedActionsJson) ?? new List<ParsedActionDto>(),
                                    l.CreatedAt,
                                    l.IssuedBy?.Username ?? "unknown"
                                )).ToList();

            return new PagedResult<ForkliftCommandDto>(page, size, total, items);
        }
    }
}
