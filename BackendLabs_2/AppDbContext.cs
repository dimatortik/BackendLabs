using BackendLabs_2.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendLabs_2;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Record> Records { get; set; } = null!;
    public DbSet<Currency> Currencies { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>()
            .HasData(new Currency()
            {
                Id = 1,
                Name = "American Dollar",
                Symbol = "USD"
            });
        
        modelBuilder.Entity<Currency>()
            .HasData(new Currency()
            {
                Id = 2,
                Name = "Euro",
                Symbol = "EUR"
            });
        
        modelBuilder.Entity<Currency>()
            .HasData(new Currency()
            {
                Id = 3,
                Name = "British Pound Sterling",
                Symbol = "GBP"
            });

        modelBuilder.Entity<Currency>()
            .HasData(new Currency()
            {
                Id = 4,
                Name = "Japanese Yen",
                Symbol = "JPY"
            });
        
        modelBuilder.Entity<Currency>()
            .HasData(new Currency()
            {
                Id = 5,
                Name = "Swiss Franc",
                Symbol = "CHF"
            })
           ;
 
    }

}