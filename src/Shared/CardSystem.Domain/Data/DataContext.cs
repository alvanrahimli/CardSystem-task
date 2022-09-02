using CardSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CardSystem.Domain.Data;

public class DataContext : DbContext
{
    public DbSet<AppUser> Users { get; set; } = default!;
    public DbSet<Card> Cards { get; set; } = default!;
    public DbSet<Account> Accounts { get; set; } = default!;
    public DbSet<Transaction> Transactions { get; set; } = default!;
    public DbSet<Vendor> Vendors { get; set; } = default!;
    
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}