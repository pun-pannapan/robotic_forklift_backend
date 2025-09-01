using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Robotic.Forklift.Application.Mappings;
using Robotic.Forklift.Infrastructure.Data;

namespace Robotic.Forklift.Backend.Tests;

public static class TestSettings
{
    public static AppDbContext NewDb(string? name = null)
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;
        return new AppDbContext(opts);
    }

    public static IMapper NewMapper()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
        return cfg.CreateMapper();
    }
}
