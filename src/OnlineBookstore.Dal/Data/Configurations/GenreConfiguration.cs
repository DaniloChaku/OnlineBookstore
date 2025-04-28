using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineBookstore.Dal.Entities;

namespace OnlineBookstore.Dal.Data.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(a => a.Id)
               .ValueGeneratedOnAdd();

        builder
            .Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
