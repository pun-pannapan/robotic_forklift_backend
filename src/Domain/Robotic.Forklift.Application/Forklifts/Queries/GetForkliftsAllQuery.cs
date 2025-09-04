using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Robotic.Forklift.Application.Dtos;
using Robotic.Forklift.Application.Interfaces;

namespace Robotic.Forklift.Application.Forklifts.Queries
{
    public record GetForkliftsAllQuery(PageQuery Query) : IRequest<PagedResult<ForkliftDto>>;

    public class GetForkliftsAllHandler : IRequestHandler<GetForkliftsAllQuery, PagedResult<ForkliftDto>>
    {
        private readonly IAppDbContext _db;
        private readonly IMapper _mapper;
        public GetForkliftsAllHandler(IAppDbContext db, IMapper mapper) 
        { 
            _db = db;
            _mapper = mapper; 
        }

        public async Task<PagedResult<ForkliftDto>> Handle(GetForkliftsAllQuery request, CancellationToken ct)
        {
            var (page, size, sortBy, dir, search) = request.Query;
            var forkLifts = _db.Forklifts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim();
                forkLifts = forkLifts.Where(x =>
                    EF.Functions.Like(x.Name, $"%{q}%") ||
                    EF.Functions.Like(x.ModelNumber, $"%{q}%")
                );
            }

            bool desc = (dir?.ToLowerInvariant() == "desc");
            switch (sortBy?.ToLowerInvariant())
            {
                case "modelnumber":
                    forkLifts = desc
                        ? forkLifts.OrderByDescending(x => x.ModelNumber)
                        : forkLifts.OrderBy(x => x.ModelNumber);
                    break;
                case "manufacturingdate":
                    forkLifts = desc
                        ? forkLifts.OrderByDescending(x => x.ManufacturingDate)
                        : forkLifts.OrderBy(x => x.ManufacturingDate);
                    break;
                default:
                    forkLifts = desc
                        ? forkLifts.OrderByDescending(x => x.Id)
                        : forkLifts.OrderBy(x => x.Id);
                    break;
            }

            var total = await forkLifts.CountAsync(ct);
            var items = await forkLifts.Skip((page - 1) * size).Take(size)
            .ProjectTo<ForkliftDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

            return new PagedResult<ForkliftDto> (page, size, total, items);
        }
    }
}
