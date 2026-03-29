using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Prictionary.Models;

namespace Prictionary.Database;
public class PrictionaryContext : IdentityDbContext<AppUser>
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<LanguageUnit> LanguageUnits { get; set; }
    public DbSet<Meaning> Meanings { get; set; }

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
                    .HasForeignKey(lug => lug.LanguageUnitID),
                l => l
                    .HasOne(e => e.Group)
                    .WithMany(e => e.LanguageUnitGroups)
                    .HasForeignKey(lug => lug.GroupID),
                languageUnitGroups =>
                {
                    languageUnitGroups.HasKey(lug => new { lug.GroupID, lug.LanguageUnitID });
                    languageUnitGroups.ToTable("LanguageUnitGroups");
                }
            );
    }
}