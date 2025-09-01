using Microsoft.EntityFrameworkCore;
using Robotic.Forklift.Domain.Entities;

namespace Robotic.Forklift.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Domain.Entities.Forklift> Forklifts { get; }
        DbSet<ForkliftCommand> ForkliftCommands { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}