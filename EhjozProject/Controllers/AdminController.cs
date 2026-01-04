using EhjozProject.Application.Interfaces;
using EhjozProject.Domain.Models.Identity;
using EhjozProject.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EhjozProject.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStadiumService _stadiumService;
        private readonly IBookingService _bookingService;
        private readonly ISubscriptionService _subscriptionService;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            IStadiumService stadiumService,
            IBookingService bookingService,
            ISubscriptionService subscriptionService)
        {
            _userManager = userManager;
            _stadiumService = stadiumService;
            _bookingService = bookingService;
            _subscriptionService = subscriptionService;
        }

        private bool IsAdmin(ApplicationUser? user)
        {
            return user != null && user.Role == "Admin";
        }

        #region Dashboard

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var allUsers = await _userManager.Users.ToListAsync();
            var allOwners = allUsers.Where(u => u.Role == "Owner").ToList();
            var allCustomers = allUsers.Where(u => u.Role == "Customer").ToList();
            var allStadiums = await _stadiumService.GetAllStadiumsAsync();
            var allBookings = await _bookingService.GetAllBookingsAsync();
            var allSubscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            var allPlans = await _subscriptionService.GetAllSubscriptionPlansAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = allUsers.Count,
                TotalOwners = allOwners.Count,
                TotalCustomers = allCustomers.Count,
                PendingOwnerApprovals = allOwners.Count(o => o.IsApproved != true),
                TotalStadiums = allStadiums.Count(),
                ActiveStadiums = allStadiums.Count(s => s.IsActive),
                TotalBookings = allBookings.Count(),
                TotalSubscriptions = allSubscriptions.Count(),
                ActiveSubscriptions = allSubscriptions.Count(s => s.IsActive),
                TotalPlans = allPlans.Count()
            };

            return View(viewModel);
        }

        #endregion

        #region Admins Management

        [HttpGet]
        public async Task<IActionResult> Admins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var allUsers = await _userManager.Users.ToListAsync();
            var admins = allUsers.Where(u => u.Role == "Admin").ToList();

            var vm = new AdminsManagementViewModel
            {
                Admins = admins.Select(a => new AdminUserViewModel
                {
                    Id = a.Id,
                    FullName = a.FullName ?? "N/A",
                    Email = a.Email ?? "N/A",
                    City = a.City
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(AdminsManagementViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Admins));
            }

            var email = model.CreateAdmin.Email.Trim();
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                TempData["Success"] = "User already exists. If you want to promote them to Admin, use the Promote action.";
                return RedirectToAction(nameof(Admins));
            }

            var newAdmin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = string.IsNullOrWhiteSpace(model.CreateAdmin.FullName) ? "Admin User" : model.CreateAdmin.FullName,
                City = model.CreateAdmin.City,
                Role = "Admin",
                IsApproved = true
            };

            var result = await _userManager.CreateAsync(newAdmin, model.CreateAdmin.Password);
            if (!result.Succeeded)
            {
                TempData["Success"] = $"Failed to create admin: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                return RedirectToAction(nameof(Admins));
            }

            TempData["Success"] = "Admin account created successfully.";
            return RedirectToAction(nameof(Admins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var target = await _userManager.FindByIdAsync(id);
            if (target == null)
            {
                return NotFound();
            }

            target.Role = "Admin";
            target.IsApproved = true;
            await _userManager.UpdateAsync(target);

            TempData["Success"] = $"User {target.Email} promoted to Admin.";
            return RedirectToAction(nameof(Admins));
        }

        #endregion

        #region Users Management

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var allUsers = await _userManager.Users.ToListAsync();
            var customers = allUsers.Where(u => u.Role == "Customer").ToList();

            var viewModel = new UserManagementViewModel
            {
                Users = customers.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName ?? "N/A",
                    Email = u.Email ?? "N/A",
                    PhoneNumber = u.PhoneNumber ?? "N/A",
                    City = u.City ?? "N/A",
                    CreatedDate = u.Id, // Using Id as proxy since we don't have CreatedDate field
                    IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserLock(string id)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (!IsAdmin(admin))
            {
                return RedirectToAction("Index", "Home");
            }

            var target = await _userManager.FindByIdAsync(id);
            if (target == null || target.Role != "Customer")
            {
                return NotFound();
            }

            target.LockoutEnabled = true;
            var isLocked = target.LockoutEnd.HasValue && target.LockoutEnd.Value > DateTimeOffset.UtcNow;
            target.LockoutEnd = isLocked ? null : DateTimeOffset.UtcNow.AddYears(100);
            await _userManager.UpdateAsync(target);

            TempData["Success"] = isLocked ? $"User {target.Email} unlocked." : $"User {target.Email} deactivated (locked).";
            return RedirectToAction(nameof(Users));
        }

        #endregion

        #region Owners Management

        [HttpGet]
        public async Task<IActionResult> Owners()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var allUsers = await _userManager.Users.ToListAsync();
            var owners = allUsers.Where(u => u.Role == "Owner").ToList();

            var viewModel = new OwnerManagementViewModel
            {
                Owners = owners.Select(o => new OwnerViewModel
                {
                    Id = o.Id,
                    FullName = o.FullName ?? "N/A",
                    Email = o.Email ?? "N/A",
                    PhoneNumber = o.PhoneNumber ?? "N/A",
                    City = o.City ?? "N/A",
                    IsApproved = o.IsApproved ?? false,
                    SubscriptionEndDate = o.SubscriptionEndDate,
                    IsLocked = o.LockoutEnd.HasValue && o.LockoutEnd.Value > DateTimeOffset.UtcNow
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOwner(string id)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (!IsAdmin(admin))
            {
                return RedirectToAction("Index", "Home");
            }

            var owner = await _userManager.FindByIdAsync(id);
            if (owner == null || owner.Role != "Owner")
            {
                return NotFound();
            }

            owner.IsApproved = true;
            await _userManager.UpdateAsync(owner);

            TempData["Success"] = $"Owner {owner.FullName} has been approved successfully!";
            return RedirectToAction(nameof(Owners));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectOwner(string id)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (!IsAdmin(admin))
            {
                return RedirectToAction("Index", "Home");
            }

            var owner = await _userManager.FindByIdAsync(id);
            if (owner == null || owner.Role != "Owner")
            {
                return NotFound();
            }

            owner.IsApproved = false;
            await _userManager.UpdateAsync(owner);

            TempData["Success"] = $"Owner {owner.FullName} has been rejected.";
            return RedirectToAction(nameof(Owners));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleOwnerLock(string id)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (!IsAdmin(admin))
            {
                return RedirectToAction("Index", "Home");
            }

            var target = await _userManager.FindByIdAsync(id);
            if (target == null || target.Role != "Owner")
            {
                return NotFound();
            }

            target.LockoutEnabled = true;
            var isLocked = target.LockoutEnd.HasValue && target.LockoutEnd.Value > DateTimeOffset.UtcNow;
            target.LockoutEnd = isLocked ? null : DateTimeOffset.UtcNow.AddYears(100);
            await _userManager.UpdateAsync(target);

            TempData["Success"] = isLocked ? $"Owner {target.Email} unlocked." : $"Owner {target.Email} deactivated (locked).";
            return RedirectToAction(nameof(Owners));
        }

        #endregion

        #region Stadiums Management

        [HttpGet]
        public async Task<IActionResult> Stadiums()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var allStadiums = await _stadiumService.GetAllStadiumsAsync();
            var viewModel = new StadiumManagementViewModel
            {
                Stadiums = allStadiums.Select(s => new StadiumViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    City = s.City,
                    Address = s.Address,
                    PricePerHour = s.PricePerHour,
                    IsActive = s.IsActive,
                    OwnerName = s.Owner?.FullName ?? s.Owner?.Email ?? "N/A"
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStadiumStatus(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var stadium = await _stadiumService.GetStadiumByIdAsync(id);
            if (stadium == null)
            {
                return NotFound();
            }

            var oldStatus = stadium.IsActive;
            await _stadiumService.ToggleStadiumStatusAsync(id);
            TempData["Success"] = $"Stadium {stadium.Name} status has been {(oldStatus ? "deactivated" : "activated")}.";
            return RedirectToAction(nameof(Stadiums));
        }

        #endregion

        #region Bookings Management

        [HttpGet]
        public async Task<IActionResult> Bookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var allBookings = await _bookingService.GetAllBookingsAsync();
            var viewModel = new BookingManagementViewModel
            {
                Bookings = allBookings.Select(b => new BookingViewModel
                {
                    Id = b.Id,
                    BookingDate = b.BookingDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CustomerName = b.User?.FullName ?? b.User?.Email ?? "N/A",
                    StadiumName = b.Stadium?.Name ?? "N/A",
                    Notes = b.Notes,
                    IsArchived = b.IsArchived == true
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            await _bookingService.CancelBookingAsync(id);
            TempData["Success"] = $"Booking #{id} cancelled.";
            return RedirectToAction(nameof(Bookings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            await _bookingService.ArchiveBookingAsync(id);
            TempData["Success"] = $"Booking #{id} archived.";
            return RedirectToAction(nameof(Bookings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            await _bookingService.DeletePermanentlyAsync(id);
            TempData["Success"] = $"Booking #{id} deleted permanently.";
            return RedirectToAction(nameof(Bookings));
        }

        #endregion

        #region Subscription Plans Management

        [HttpGet]
        public async Task<IActionResult> Plans()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var allPlans = await _subscriptionService.GetAllSubscriptionPlansAsync();
            var viewModel = new SubscriptionPlanManagementViewModel
            {
                Plans = allPlans.Select(p => new SubscriptionPlanViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DurationDays = p.DurationDays,
                    MaxStadiums = p.MaxStadiums,
                    IsActive = p.IsActive
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlan(SubscriptionPlanManagementViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrWhiteSpace(model.CreatePlan.Name))
            {
                TempData["Success"] = "Plan name is required.";
                return RedirectToAction(nameof(Plans));
            }

            var plan = new EhjozProject.Domain.Models.Subscription.SubscriptionPlan
            {
                Name = model.CreatePlan.Name.Trim(),
                Description = model.CreatePlan.Description,
                Price = model.CreatePlan.Price,
                DurationDays = model.CreatePlan.DurationDays,
                MaxStadiums = model.CreatePlan.MaxStadiums,
                IsActive = model.CreatePlan.IsActive
            };

            await _subscriptionService.CreateSubscriptionPlanAsync(plan);
            TempData["Success"] = "Subscription plan created.";
            return RedirectToAction(nameof(Plans));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptPlan(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var plan = await _subscriptionService.GetSubscriptionPlanByIdAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            plan.IsActive = true;
            await _subscriptionService.UpdateSubscriptionPlanAsync(plan);

            TempData["Success"] = $"Subscription plan {plan.Name} has been accepted.";
            return RedirectToAction(nameof(Plans));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivatePlan(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var plan = await _subscriptionService.GetSubscriptionPlanByIdAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            plan.IsActive = false;
            await _subscriptionService.UpdateSubscriptionPlanAsync(plan);

            TempData["Success"] = $"Subscription plan {plan.Name} has been deactivated.";
            return RedirectToAction(nameof(Plans));
        }

        #endregion

        #region Subscriptions Management

        [HttpGet]
        public async Task<IActionResult> Subscriptions()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var allSubscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            var viewModel = new SubscriptionManagementViewModel
            {
                Subscriptions = allSubscriptions.Select(s => new SubscriptionViewModel
                {
                    Id = s.Id,
                    OwnerName = s.Owner?.FullName ?? s.Owner?.Email ?? "N/A",
                    PlanName = s.Plan?.Name ?? "N/A",
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateSubscription(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin(user))
            {
                return RedirectToAction("Index", "Home");
            }

            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            subscription.IsActive = false;
            if (subscription.EndDate > DateTime.UtcNow)
            {
                subscription.EndDate = DateTime.UtcNow;
            }

            await _subscriptionService.UpdateSubscriptionAsync(subscription);

            // Best-effort: update owner's SubscriptionEndDate too
            var owner = await _userManager.FindByIdAsync(subscription.OwnerId);
            if (owner != null)
            {
                owner.SubscriptionEndDate = subscription.EndDate;
                await _userManager.UpdateAsync(owner);
            }

            TempData["Success"] = $"Subscription #{id} deactivated.";
            return RedirectToAction(nameof(Subscriptions));
        }

        #endregion
    }
}

