using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)

{
    public DbSet<Department> Departments { get; set; }

    public DbSet<Position> Positions { get; set; }

    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine);
    }
}