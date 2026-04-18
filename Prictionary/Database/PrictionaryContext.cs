using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Prictionary.Models;

namespace Prictionary.Database;
public class PrictionaryContext : IdentityDbContext<AppUser>
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<LanguageUnit> LanguageUnits { get; set; }
    public DbSet<Meaning> Meanings { get; set; }

    public DbSet<LanguageUnitGroup> LanguageUnitGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseNpgsql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>()
            .HasMany(e => e.LanguageUnits)
            .WithMany(e => e.Groups)
            .UsingEntity<LanguageUnitGroup>(
                r => r
                    .HasOne(e => e.LanguageUnit)
                    .WithMany(e => e.LanguageUnitGroups)
                    .HasForeignKey(lug => lug.LanguageUnitId),
                l => l
                    .HasOne(e => e.Group)
                    .WithMany(e => e.LanguageUnitGroups)
                    .HasForeignKey(lug => lug.GroupId),
                languageUnitGroups =>
                {
                    languageUnitGroups.HasKey(lug => new { lug.GroupId, lug.LanguageUnitId });
                    languageUnitGroups.ToTable("LanguageUnitGroups");
                }
            );
    }
}