//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace CA.Persistance.Configurations
//{

//    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
//    {
//        public void Configure(EntityTypeBuilder<Sale> builder)
//        {
//            builder.Property(p => p.Date).IsRequired();
//            builder.Property(p => p.UnitPrice).HasColumnType("decimal(5,2)");
//            builder.Property(p => p.TotalPrice).IsRequired().HasColumnType("money");
//        }
//    }
//}
