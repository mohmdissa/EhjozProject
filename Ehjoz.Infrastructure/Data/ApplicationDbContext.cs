using EhjozProject.Domain.Models.Booking;
using EhjozProject.Domain.Models.Identity;
using EhjozProject.Domain.Models.Payment;
using EhjozProject.Domain.Models.Stadium;
using EhjozProject.Domain.Models.Subscription;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EhjozProject.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - PLURAL names
        public DbSet<Stadium> Stadiums { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Stadium Configuration
            builder.Entity<Stadium>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Address).IsRequired().HasMaxLength(200);
                entity.Property(s => s.City).IsRequired().HasMaxLength(50);
                entity.Property(s => s.PricePerHour).HasColumnType("decimal(18,2)");

                entity.HasOne(s => s.Owner)
                    .WithMany(u => u.Stadiums)
                    .HasForeignKey(s => s.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TimeSlot Configuration
            builder.Entity<TimeSlot>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasOne(t => t.Stadium)
                    .WithMany(s => s.TimeSlots)
                    .HasForeignKey(t => t.StadiumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Booking Configuration
            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.TotalPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(b => b.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Stadium)
                    .WithMany(s => s.Bookings)
                    .HasForeignKey(b => b.StadiumId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.TimeSlot)
                    .WithOne(t => t.Booking)
                    .HasForeignKey<Booking>(b => b.TimeSlotId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Payment Configuration
            builder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                entity.Property(p => p.IsPaid).HasDefaultValue(false);

                entity.HasOne(p => p.Booking)
                    .WithOne(b => b.Payment)
                    .HasForeignKey<Payment>(p => p.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Subscription Configuration
            builder.Entity<Subscription>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.Owner)
                    .WithMany()
                    .HasForeignKey(s => s.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Plan)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(s => s.PlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SubscriptionPlan Configuration
            builder.Entity<SubscriptionPlan>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });
        }
    }
}