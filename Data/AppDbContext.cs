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
    public DbSet<UserModel> User { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LugarModel>()
            .HasMany(lugar => lugar.Tags)
            .WithMany(tag => tag.Lugares)
            .UsingEntity(j => j.ToTable("LugarTag"));
        modelBuilder.Entity<LugarModel>()
            .HasMany(lugar => lugar.Users)
            .WithMany(user => user.Lugares)
            .UsingEntity(j => j.ToTable("LugarUser"));
        modelBuilder.Entity<LugarModel>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<LugarModel>()
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<TagModel>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<TagModel>()
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<UserModel>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<UserModel>()
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<UserModel>()
            .HasIndex(e => e.Email)
            .IsUnique(true);
    }
    
    public override int SaveChanges()
    {
        var modifiedEntities = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);
        foreach (var entity in modifiedEntities)
        {
            entity.Property("UpdatedAt").CurrentValue = DateTime.Now;
        }

        return base.SaveChanges();
    }
}