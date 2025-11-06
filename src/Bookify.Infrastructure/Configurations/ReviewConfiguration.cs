using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(review => review.Id);
        builder.Property(x => x.Id).HasConversion(
            (x) => x.Value,
            (g) => ReviewId.FromValue(g)
        );

        builder.Property(review => review.Rating)
            .HasConversion(rating => rating.Value, value => Rating.Create(value).Value);

        builder.Property(review => review.Comment)
            .HasMaxLength(200)
            .HasConversion(comment => comment.Value, value => new Comment(value));

        builder.Property(booking => booking.ApartmentId)
             .HasConversion(id => id.Value, value => ApartmentId.FromValue(value))
             .IsRequired();

        builder.Property(booking => booking.BookingId)
             .HasConversion(id => id.Value, value => BookingId.FromValue(value))
             .IsRequired();

        builder.Property(booking => booking.UserId)
             .HasConversion(id => id.Value, value => UserId.FromValue(value))
             .IsRequired();


        //builder.HasOne<Apartment>()
        //    .WithMany()
        //    .HasForeignKey(review => review.ApartmentId)
        //    .HasPrincipalKey(apartment => apartment.Id);

        //builder.HasOne<Booking>()
        //    .WithMany()
        //    .HasForeignKey(review => review.BookingId);

        //builder.HasOne<User>()
        //    .WithMany()
        //    .HasForeignKey(review => review.UserId);
    }
}