using CA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CA.Persistance.Configurations
{

    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.Property(p => p.Chapter).IsRequired().HasMaxLength(2);
            builder.Property(p => p.Verse).IsRequired().HasMaxLength(2);
            builder.HasIndex(p => new { p.Chapter, p.Verse }).IsUnique(true);

            builder.Property(u => u.SerialNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);


            #region Below sample code for decimal and money data type for reference
#pragma warning disable S125 // Sections of code should not be commented out
            /*
                builder.Property(p => p.UnitPrice).HasColumnType("decimal(5,2)");
                builder.Property(p => p.TotalPrice).IsRequired().HasColumnType("money");
            */
#pragma warning restore S125 // Sections of code should not be commented out
            #endregion
        }


    }
}
