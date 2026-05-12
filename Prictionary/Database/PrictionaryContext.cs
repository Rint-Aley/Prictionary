using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Prictionary.Models;

namespace Prictionary.Database;
public class PrictionaryContext : IdentityDbContext<AppUser>
{
    public PrictionaryContext(DbContextOptions<PrictionaryContext> options) : base(options) { }

    public DbSet<Group> Groups { get; set; }
    public DbSet<LanguageUnit> LanguageUnits { get; set; }
    public DbSet<Meaning> Meanings { get; set; }

    public DbSet<LanguageUnitGroup> LanguageUnitGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrictionaryContext).Assembly);
    }
}