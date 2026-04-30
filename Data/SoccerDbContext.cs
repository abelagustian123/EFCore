using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Data;

public class SoccerDbContext : DbContext
{
    public DbSet<League> Leagues { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Player> Players { get; set; }
    
    public SoccerDbContext(DbContextOptions<SoccerDbContext> options) : base(options) { }
    
    public SoccerDbContext(){}
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=AppDatabase.db");
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}