using EhjozProject.Domain.Models.Identity;
using EhjozProject.Domain.Models.Stadium;
using EhjozProject.Domain.Models.Booking;
using EhjozProject.Domain.Models.Subscription;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EhjozProject.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Apply migrations if needed
            await context.Database.MigrateAsync();

            // Check if admin user already exists
            var adminEmail = "admin@ehjoz.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator",
                    Role = "Admin",
                    IsApproved = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Ensure existing user has Admin role
                if (adminUser.Role != "Admin")
                {
                    adminUser.Role = "Admin";
                    adminUser.IsApproved = true;
                    await userManager.UpdateAsync(adminUser);
                }
                
                // Reset password to ensure it's correct
                var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                var resetResult = await userManager.ResetPasswordAsync(adminUser, token, "Admin123!");
                if (!resetResult.Succeeded)
                {
                    // If reset fails, remove password and set it again
                    await userManager.RemovePasswordAsync(adminUser);
                    await userManager.AddPasswordAsync(adminUser, "Admin123!");
                }
            }
        }

        /// <summary>
        /// Seeds demo/sample data for easier UI testing (Development only; guarded by config in Program.cs).
        /// Safe to run multiple times: it only seeds when core tables are empty.
        /// </summary>
        public static async Task SeedDemoDataAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure schema exists
            await context.Database.MigrateAsync();

            // If there's already meaningful data, do nothing
            var hasStadiums = await context.Stadiums.AnyAsync();
            var hasBookings = await context.Bookings.AnyAsync();
            var hasPlans = await context.SubscriptionPlans.AnyAsync();
            if (hasStadiums || hasBookings || hasPlans)
            {
                return;
            }

            // --- Plans ---
            var basicPlan = new SubscriptionPlan
            {
                Name = "Basic",
                Description = "1 stadium, 30 days",
                Price = 49,
                DurationDays = 30,
                MaxStadiums = 1,
                IsActive = true
            };
            var proPlan = new SubscriptionPlan
            {
                Name = "Pro",
                Description = "3 stadiums, 90 days",
                Price = 119,
                DurationDays = 90,
                MaxStadiums = 3,
                IsActive = true
            };
            var enterprisePlan = new SubscriptionPlan
            {
                Name = "Enterprise",
                Description = "Unlimited stadiums, 365 days",
                Price = 399,
                DurationDays = 365,
                MaxStadiums = 999,
                IsActive = false
            };

            context.SubscriptionPlans.AddRange(basicPlan, proPlan, enterprisePlan);
            await context.SaveChangesAsync();

            // --- Owners (one approved, one pending) ---
            var owner1Email = "owner1@ehjoz.com";
            var owner2Email = "owner2@ehjoz.com";

            var owner1 = await userManager.FindByEmailAsync(owner1Email);
            if (owner1 == null)
            {
                owner1 = new ApplicationUser
                {
                    UserName = owner1Email,
                    Email = owner1Email,
                    EmailConfirmed = true,
                    FullName = "Omar Owner",
                    City = "Riyadh",
                    Role = "Owner",
                    IsApproved = true
                };
                await userManager.CreateAsync(owner1, "Owner123!");
            }

            var owner2 = await userManager.FindByEmailAsync(owner2Email);
            if (owner2 == null)
            {
                owner2 = new ApplicationUser
                {
                    UserName = owner2Email,
                    Email = owner2Email,
                    EmailConfirmed = true,
                    FullName = "Sara Owner",
                    City = "Jeddah",
                    Role = "Owner",
                    IsApproved = false
                };
                await userManager.CreateAsync(owner2, "Owner123!");
            }

            // --- Customers ---
            var customerEmails = new[]
            {
                "user1@ehjoz.com",
                "user2@ehjoz.com",
                "user3@ehjoz.com",
                "user4@ehjoz.com",
                "user5@ehjoz.com"
            };

            var customers = new List<ApplicationUser>();
            foreach (var email in customerEmails)
            {
                var u = await userManager.FindByEmailAsync(email);
                if (u == null)
                {
                    u = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        FullName = $"Customer {email.Split('@')[0].ToUpperInvariant()}",
                        City = "Riyadh",
                        Role = "Customer",
                        IsApproved = true
                    };
                    await userManager.CreateAsync(u, "User123!");
                }
                customers.Add(u);
            }

            // --- Subscriptions ---
            var now = DateTime.UtcNow;
            context.Subscriptions.AddRange(
                new Subscription
                {
                    OwnerId = owner1.Id,
                    PlanId = proPlan.Id,
                    StartDate = now.AddDays(-10),
                    EndDate = now.AddDays(80),
                    IsActive = true
                },
                new Subscription
                {
                    OwnerId = owner2.Id,
                    PlanId = basicPlan.Id,
                    StartDate = now.AddDays(-40),
                    EndDate = now.AddDays(-10),
                    IsActive = false
                }
            );

            owner1.SubscriptionEndDate = now.AddDays(80);
            owner2.SubscriptionEndDate = now.AddDays(-10);
            await userManager.UpdateAsync(owner1);
            await userManager.UpdateAsync(owner2);

            await context.SaveChangesAsync();

            // --- Stadiums ---
            var stadiums = new List<Stadium>
            {
                new Stadium
                {
                    Name = "Green Arena",
                    Description = "Premium grass field with night lights",
                    Address = "King Fahd Rd",
                    City = "Riyadh",
                    PricePerHour = 120,
                    ImageUrl = "https://images.unsplash.com/photo-1521412644187-c49fa049e84d?auto=format&fit=crop&w=1200&q=80",
                    OwnerId = owner1.Id,
                    IsActive = true
                },
                new Stadium
                {
                    Name = "City Sports Field",
                    Description = "Great for 5v5 and 7v7",
                    Address = "Olaya Street",
                    City = "Riyadh",
                    PricePerHour = 90,
                    ImageUrl = "https://images.unsplash.com/photo-1518091043644-c1d4457512c6?auto=format&fit=crop&w=1200&q=80",
                    OwnerId = owner1.Id,
                    IsActive = true
                },
                new Stadium
                {
                    Name = "Coastal Arena",
                    Description = "By the sea — amazing vibes",
                    Address = "Corniche Rd",
                    City = "Jeddah",
                    PricePerHour = 110,
                    ImageUrl = "https://images.unsplash.com/photo-1546519638-68e109498ffc?auto=format&fit=crop&w=1200&q=80",
                    OwnerId = owner2.Id,
                    IsActive = false
                }
            };

            context.Stadiums.AddRange(stadiums);
            await context.SaveChangesAsync();

            // --- TimeSlots + Bookings ---
            var today = DateOnly.FromDateTime(DateTime.Today);
            var slotStartTimes = new[] { new TimeOnly(17, 0), new TimeOnly(19, 0), new TimeOnly(21, 0) };

            var timeSlots = new List<TimeSlot>();
            foreach (var s in stadiums)
            {
                for (var d = 0; d < 3; d++)
                {
                    var date = today.AddDays(d);
                    foreach (var st in slotStartTimes)
                    {
                        timeSlots.Add(new TimeSlot
                        {
                            StadiumId = s.Id,
                            Date = date,
                            StartTime = st,
                            EndTime = st.AddHours(2),
                            IsAvailable = true
                        });
                    }
                }
            }

            context.TimeSlots.AddRange(timeSlots);
            await context.SaveChangesAsync();

            // Create a few bookings using existing timeslots
            var bookable = await context.TimeSlots
                .OrderBy(t => t.Id)
                .Take(6)
                .ToListAsync();

            var statuses = new[] { "Confirmed", "Pending", "Cancelled" };
            var bookings = new List<Booking>();
            for (var i = 0; i < bookable.Count; i++)
            {
                var ts = bookable[i];
                var stadium = stadiums.First(st => st.Id == ts.StadiumId);
                var customer = customers[i % customers.Count];

                bookings.Add(new Booking
                {
                    UserId = customer.Id,
                    StadiumId = ts.StadiumId,
                    TimeSlotId = ts.Id,
                    BookingDate = ts.Date.ToDateTime(ts.StartTime),
                    TotalPrice = stadium.PricePerHour,
                    Status = statuses[i % statuses.Length],
                    Notes = (i % 2 == 0) ? "Seeded demo booking" : null
                });

                // Mark slot as no longer available when booked
                ts.IsAvailable = false;
            }

            context.Bookings.AddRange(bookings);
            await context.SaveChangesAsync();
        }
    }
}

