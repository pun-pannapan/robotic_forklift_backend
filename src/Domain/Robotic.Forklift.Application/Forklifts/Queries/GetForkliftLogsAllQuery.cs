using MediatR;
using Microsoft.EntityFrameworkCore;
using Robotic.Forklift.Application.Dtos;
using Robotic.Forklift.Application.Interfaces;
using System.Text.Json;

namespace Robotic.Forklift.Application.Forklifts.Queries
{
    public record GetForkliftLogsAllQuery(PageQuery Query) : IRequest<PagedResult<ForkliftCommandDto>>;

    public class GetForkliftLogsAllQueryHandler : IRequestHandler<GetForkliftLogsAllQuery, PagedResult<ForkliftCommandDto>>
    {
        private readonly IAppDbContext _db;
        public GetForkliftLogsAllQueryHandler(IAppDbContext db) 
        { 
            _db = db; 
        }

        public async Task<PagedResult<ForkliftCommandDto>> Handle(GetForkliftLogsAllQuery request, CancellationToken ct)
        {
            var (page, size, sortBy, dir) = request.Query;
            var forkliftCommand = _db.ForkliftCommands
                                        .Include(l => l.IssuedBy)
                                        .AsNoTracking();

            bool desc = dir?.ToLowerInvariant() == "desc";
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

            var items = list.Select(item => new ForkliftCommandDto(
                                item.Id,
                                item.ForkliftId,
                                item.Command,
                                JsonSerializer.Deserialize<List<ParsedActionDto>>(item.ParsedActionsJson) ?? new List<ParsedActionDto>(),
                                item.CreatedAt,
                                item.IssuedBy?.Username ?? "unknown"
                            )).ToList();

            return new PagedResult<ForkliftCommandDto>(page, size, total, items);
        }
    }
}
