using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prictionary.Models;

namespace Prictionary.Database.Configurations;

public class MeaningConfiguration : IEntityTypeConfiguration<Meaning>
{
    public void Configure(EntityTypeBuilder<Meaning> builder)
    {
        builder.ToTable("Meanings");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Priority)
            .HasDefaultValue(0);

        builder.HasOne(m => m.LanguageUnit)
            .WithMany(lu => lu.Meanings)
            .HasForeignKey(m => m.LanguageUnitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
