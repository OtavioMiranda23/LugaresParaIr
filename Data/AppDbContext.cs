using LugaresParaIr.Models;
using Microsoft.EntityFrameworkCore;

namespace LugaresParaIr.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<LugarModel> Lugares { get; set; }
    public DbSet<TagModel> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LugarModel>()
            .HasMany(lugar => lugar.Tags)
            .WithMany(tag => tag.Lugares)
            .UsingEntity(j => j.ToTable("LugarTag"));
    }
}