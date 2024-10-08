using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public class ForecastDbContext : DbContext
{
    public ForecastDbContext(DbContextOptions<ForecastDbContext> options) : base(options) { }

    public DbSet<Forecast> Forecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Forecast>().ToTable("Forecast");
    }
}
