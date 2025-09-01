using Microsoft.EntityFrameworkCore;
using Robotic.Forklift.Application.Interfaces;
using Robotic.Forklift.Domain.Entities;
using ForkliftEntity = Robotic.Forklift.Domain.Entities.Forklift;
    
namespace Robotic.Forklift.Infrastructure.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<ForkliftEntity> Forklifts => Set<ForkliftEntity>();
        public DbSet<ForkliftCommand> ForkliftCommands => Set<ForkliftCommand>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
            modelBuilder.Entity<ForkliftEntity>().Property(x => x.Name).HasMaxLength(200).IsRequired();
            modelBuilder.Entity<ForkliftEntity>().Property(x => x.ModelNumber).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<ForkliftCommand>().HasOne(x => x.Forklift).WithMany(f => f.Commands).HasForeignKey(x => x.ForkliftId);
            modelBuilder.Entity<ForkliftCommand>().HasOne(x => x.IssuedBy).WithMany().HasForeignKey(x => x.IssuedByUserId);
        }
    }
}
