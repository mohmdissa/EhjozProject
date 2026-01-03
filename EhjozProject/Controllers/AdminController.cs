using EhjozProject.Application.Interfaces;
using EhjozProject.Domain.Models.Identity;
using EhjozProject.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EhjozProject.Web.Controllers
{
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
                    CreatedDate = u.Id // Using Id as proxy since we don't have CreatedDate field
                }).ToList()
            };

            return View(viewModel);
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
                    SubscriptionEndDate = o.SubscriptionEndDate
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
                    Notes = b.Notes
                }).ToList()
            };

            return View(viewModel);
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

        #endregion
    }
}

