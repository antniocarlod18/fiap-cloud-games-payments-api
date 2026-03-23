using FiapCloudGamesPayments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGamesPayments.Infra.Data.Configurations;

internal class OrderPaymentConfiguration : IEntityTypeConfiguration<OrderPayment>
{
    public void Configure(EntityTypeBuilder<OrderPayment> builder)
    {
        builder.ToTable("OrderPayment");
        builder.HasKey(u => u.Id);
        builder.Property(p => p.DateCreated).HasColumnType("DATETIME").IsRequired();
        builder.Property(p => p.DateUpdated).HasColumnType("DATETIME");

        builder.Property(u => u.UserId)
            .IsRequired();

        builder.Property(u => u.OrderId)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired();

        builder.Property(o => o.Method)
            .IsRequired();

        builder.Property(o => o.Currency)
            .IsRequired();

        builder.Property(o => o.Price)
            .IsRequired()
            .HasColumnType("decimal(10,2)");
    }
}
