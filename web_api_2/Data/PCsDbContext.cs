using Microsoft.EntityFrameworkCore;
using web_api_2.Models;

namespace web_api_2.Data;

public class PCsDbContext : DbContext
{
    public PCsDbContext(DbContextOptions<PCsDbContext> options) : base(options)
    {
    }

    public DbSet<Pc> PCs { get; set; }
    public DbSet<Component> Components { get; set; }
    public DbSet<ComponentType> ComponentTypes { get; set; }
    public DbSet<ComponentManufacturer> ComponentManufacturers { get; set; }
    public DbSet<PcComponent> PcComponents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Pc>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Weight).IsRequired();
            entity.Property(e => e.Warranty).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Stock).IsRequired();

            entity.HasData(
                new Pc { Id = 1, Name = "Professional PC", Weight = 8.5, Warranty = 24, CreatedAt = new DateTime(2026, 5, 8, 9, 0, 0), Stock = 5 },
                new Pc { Id = 2, Name = "Home Office PC", Weight = 4.2, Warranty = 24, CreatedAt = new DateTime(2026, 4, 15, 13, 30, 0), Stock = 12 },
                new Pc { Id = 3, Name = "Student PC", Weight = 6.1, Warranty = 12, CreatedAt = new DateTime(2026, 3, 10, 10, 0, 0), Stock = 20 }
            );
        });

        
        modelBuilder.Entity<ComponentType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Abbreviation).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();

            entity.HasData(
                new ComponentType { Id = 1, Abbreviation = "CPU", Name = "Procesor" },
                new ComponentType { Id = 2, Abbreviation = "GPU", Name = "Graphic" },
                new ComponentType { Id = 3, Abbreviation = "SSD", Name = "Disk" }
            );
        });
        
        

        modelBuilder.Entity<ComponentManufacturer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Abbreviation).HasMaxLength(30).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(300).IsRequired();
            entity.Property(e => e.FoundationDate).IsRequired();

            entity.HasData(
                new ComponentManufacturer { Id = 1, Abbreviation = "AMD", FullName = "Advanced Micro Devices", FoundationDate = new DateTime(1991, 2, 22) },
                new ComponentManufacturer { Id = 2, Abbreviation = "IN", FullName = "Intel", FoundationDate = new DateTime(2001, 2, 1) },
                new ComponentManufacturer { Id = 3, Abbreviation = "NV", FullName = "Nvidia", FoundationDate = new DateTime(1994, 4, 15) }
            );
        });
        
        

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Description).IsRequired();

            entity.HasOne(e => e.Manufacturer)
                .WithMany(e => e.Components)
                .HasForeignKey(e => e.ComponentManufacturerId);

            entity.HasOne(e => e.Type)
                .WithMany(e => e.Components)
                .HasForeignKey(e => e.ComponentTypeId);

            entity.HasData(
                new Component { Code = "CPU/000111", Name = "Ryzen 7", Description = "Gaming processor", ComponentManufacturerId = 1, ComponentTypeId = 1 },
                new Component { Code = "GPU/000222", Name = "RTX 4090", Description = "gaming graphics card", ComponentManufacturerId = 2, ComponentTypeId = 2 },
                new Component { Code = "RAM/000333", Name = "16GB", Description = "DDR5 RAM module 16GB", ComponentManufacturerId = 3, ComponentTypeId = 3 }
            );
        });
        
        

        modelBuilder.Entity<PcComponent>(entity =>
        {
            entity.HasKey(e => new { e.PCId, e.ComponentCode });
            entity.Property(e => e.ComponentCode).HasMaxLength(10);
            entity.Property(e => e.Amount).IsRequired();
            entity.HasOne(e => e.PC)
                .WithMany(e => e.PcComponents)
                .HasForeignKey(e => e.PCId);
            entity.HasOne(e => e.Component)
                .WithMany(e => e.PcComponents)
                .HasForeignKey(e => e.ComponentCode);

            entity.HasData(
                new PcComponent { PCId = 1, ComponentCode = "CPU/000111", Amount = 1 },
                new PcComponent { PCId = 1, ComponentCode = "GPU/000222", Amount = 1 },
                new PcComponent { PCId = 1, ComponentCode = "RAM/000333", Amount = 2 },
                new PcComponent { PCId = 2, ComponentCode = "CPU/000111", Amount = 1 },
                new PcComponent { PCId = 3, ComponentCode = "RAM/000333", Amount = 1 }
            );
        });
    }
}