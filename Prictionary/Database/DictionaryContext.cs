using Microsoft.EntityFrameworkCore;
using Prictionary.Models;

namespace Prictionary.Database;
public class DictionaryContext : DbContext
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<LanguageUnit> LanguageUnits { get; set; }
    public DbSet<Translation> Translations { get; set; }
    public DbSet<LanguageUnitGroup> languageUnitGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseNpgsql();
    }
}