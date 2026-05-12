using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prictionary.Models;

namespace Prictionary.Database.Configurations;

public class LanguageUnitConfiguration : IEntityTypeConfiguration<LanguageUnit>
{
    public void Configure(EntityTypeBuilder<LanguageUnit> builder)
    {
        builder.ToTable("LanguageUnits");

        builder.HasKey(lu => lu.Id);

        builder.Property(lu => lu.Content)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(lu => lu.AdditionalInformation)
            .HasMaxLength(400);

        builder.Property(lu => lu.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(lu => lu.LastModifiedAt)
            .HasDefaultValueSql("now()");
    }
}
