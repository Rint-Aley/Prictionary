using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prictionary.Models;

namespace Prictionary.Database.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .HasMaxLength(500);

        builder.Property(g => g.OwnerId)
            .IsRequired();

        builder.HasOne(g => g.Owner)
            .WithMany()
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(g => g.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(g => g.LastWordAddedAt)
            .HasDefaultValueSql("now()");

        builder.HasMany(g => g.LanguageUnits)
            .WithMany(lu => lu.Groups)
            .UsingEntity<LanguageUnitGroup>(
                r => r
                    .HasOne(e => e.LanguageUnit)
                    .WithMany(e => e.LanguageUnitGroups)
                    .HasForeignKey(lug => lug.LanguageUnitId),
                l => l
                    .HasOne(e => e.Group)
                    .WithMany(e => e.LanguageUnitGroups)
                    .HasForeignKey(lug => lug.GroupId),
                lug =>
                {
                    lug.ToTable("LanguageUnitGroups");
                    lug.HasKey(e => new { e.GroupId, e.LanguageUnitId });
                    lug.Property(e => e.AddedAt)
                        .HasDefaultValueSql("now()");
                }
            );
    }
}
