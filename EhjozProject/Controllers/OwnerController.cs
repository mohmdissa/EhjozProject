    using EhjozProject.Application.Interfaces;
    using EhjozProject.Domain.Models.Identity;
    using EhjozProject.Domain.Models.Stadium;
    using EhjozProject.Web.ViewModels.Owner;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    namespace EhjozProject.Web.Controllers
    {
        public class OwnerController : Controller
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly IStadiumService _stadiumService;

            public OwnerController(
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                IStadiumService stadiumService)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _stadiumService = stadiumService;
            }

            #region Registration

            // GET: Owner/Register
            [HttpGet]
            public IActionResult Register()
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction(nameof(Dashboard));
                }
                return View();
            }

            // POST: Owner/Register
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Register(OwnerRegisterViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        City = model.City,
                        Address = model.Address,
                        Role = "Owner",
                        IsApproved = true
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        TempData["Success"] = "Registration successful! Welcome to Ehjoz.";
                        return RedirectToAction(nameof(Dashboard));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return View(model);
            }

            #endregion

            #region Dashboard

            // GET: Owner/Dashboard
            [Authorize]
            [HttpGet]
            public async Task<IActionResult> Dashboard()
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null || user.Role != "Owner")
                {
                    return RedirectToAction("Index", "Home");
                }

                var stadiums = await _stadiumService.GetStadiumsByOwnerIdAsync(user.Id);

                ViewBag.TotalStadiums = stadiums.Count();
                ViewBag.ActiveStadiums = stadiums.Count(s => s.IsActive);
                ViewBag.OwnerName = user.FullName ?? user.Email;

                return View(stadiums);
            }

            #endregion

            #region Stadium Management

            // GET: Owner/MyStadiums
            [Authorize]
            [HttpGet]
            public async Task<IActionResult> MyStadiums()
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null || user.Role != "Owner")
                {
                    return RedirectToAction("Index", "Home");
                }

                var stadiums = await _stadiumService.GetStadiumsByOwnerIdAsync(user.Id);
                return View(stadiums);
            }

            // GET: Owner/AddStadium
            [Authorize]
            [HttpGet]
            public async Task<IActionResult> AddStadium()
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null || user.Role != "Owner")
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(new StadiumViewModel());
            }

            // POST: Owner/AddStadium
            [Authorize]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddStadium(StadiumViewModel model)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null || user.Role != "Owner")
                {
                    return RedirectToAction("Index", "Home");
                }

                if (ModelState.IsValid)
                {
                    var stadium = new Stadium
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Address = model.Address,
                        City = model.City,
                        PricePerHour = model.PricePerHour,
                        ImageUrl = model.ImageUrl,
                        IsActive = model.IsActive,
                        OwnerId = user.Id
                    };

                    await _stadiumService.CreateStadiumAsync(stadium);

                    TempData["Success"] = "Stadium added successfully!";
                    return RedirectToAction(nameof(MyStadiums));
                }

                return View(model);
            }

            // GET: Owner/EditStadium/5
            [Authorize]
            [HttpGet]
            public async Task<IActionResult> EditStadium(int id)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null || user.Role != "Owner")
                {
                    return RedirectToAction("Index", "Home");
                }

                var stadium = await _stadiumService.GetStadiumByIdAsync(id);

                if (stadium == null || stadium.OwnerId != user.Id)
                {
                    return NotFound();
                }

                var model = new StadiumViewModel
                {
                    Id = stadium.Id,
                    Name = stadium.Name,
                    Description = stadium.Description,
                    Address = stadium.Address,
                    City = stadium.City,
                    PricePerHour = stadium.PricePerHour,
                    ImageUrl = stadium.ImageUrl,
                    IsActive = stadium.IsActive
                };

                return View(model);
            }

            // POST: Owner/EditStadium/5
            [Authorize]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> EditStadium(int id, StadiumViewModel model)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null || user.Role != "Owner")
                {
                    return RedirectToAction("Index", "Home");
                }

                if (id != model.Id)
                {
                    return BadRequest();
                }

                var stadium = await _stadiumService.GetStadiumByIdAsync(id);

                if (stadium == null || stadium.OwnerId != user.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    stadium.Name = model.Name;
                    stadium.Description = model.Description;
                    stadium.Address = model.Address;
                    stadium.City = model.City;
                    stadium.PricePerHour = model.PricePerHour;
                    stadium.ImageUrl = model.ImageUrl;
                    stadium.IsActive = model.IsActive;

                    await _stadiumService.UpdateStadiumAsync(stadium);

                    TempData["Success"] = "Stadium updated successfully!";
                    return RedirectToAction(nameof(MyStadiums));
                }

                return View(model);
            }

            // POST: Owner/DeleteStadium/5
            [Authorize]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteStadium(int id)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null || user.Role != "Owner")
                {
                    return RedirectToAction("Index", "Home");
                }

                var stadium = await _stadiumService.GetStadiumByIdAsync(id);

                if (stadium == null || stadium.OwnerId != user.Id)
                {
                    return NotFound();
                }

                await _stadiumService.DeleteStadiumAsync(id);

                TempData["Success"] = "Stadium deleted successfully!";
                return RedirectToAction(nameof(MyStadiums));
            }

            // POST: Owner/ToggleStatus/5
            [Authorize]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ToggleStatus(int id)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null || user.Role != "Owner")
                {
                    return RedirectToAction("Index", "Home");
                }

                var stadium = await _stadiumService.GetStadiumByIdAsync(id);

                if (stadium == null || stadium.OwnerId != user.Id)
                {
                    return NotFound();
                }

                await _stadiumService.ToggleStadiumStatusAsync(id);

                TempData["Success"] = stadium.IsActive ? "Stadium deactivated!" : "Stadium activated!";
                return RedirectToAction(nameof(MyStadiums));
            }

            #endregion

        


        // POST: Owner/DeleteStadium/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteStadium(int id)
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    if (user == null || user.Role != "Owner")
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    var stadium = await _stadiumService.GetStadiumByIdAsync(id);

        //    if (stadium == null || stadium.OwnerId != user.Id)
        //    {
        //        return NotFound();
        //    }

        //    await _stadiumService.DeleteStadiumAsync(id);

        //    TempData["Success"] = "Stadium deleted successfully!";
        //    return RedirectToAction(nameof(MyStadiums));
        //}

        // POST: Owner/ToggleStatus/5
        [Authorize]
        [HttpPost]
        
        //public async Task<IActionResult> ToggleStatus(int id)
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    if (user == null || user.Role != "Owner")
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    var stadium = await _stadiumService.GetStadiumByIdAsync(id);

        //    if (stadium == null || stadium.OwnerId != user.Id)
        //    {
        //        return NotFound();
        //    }

        //    await _stadiumService.ToggleStadiumStatusAsync(id);

        //    TempData["Success"] = stadium.IsActive ? "Stadium deactivated!" : "Stadium activated!";
        //    return RedirectToAction(nameof(MyStadiums));
        //}
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Subscription()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.Role != "Owner")
            {
                return RedirectToAction("Index", "Home");
            }

            // For now, just return the view (subscription service not implemented yet)
            ViewBag.Subscription = null;
            ViewBag.Plans = null;

            return View();
        }

        // GET: Owner/TimeSlots
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> TimeSlots(int stadiumId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.Role != "Owner")
            {
                return RedirectToAction("Index", "Home");
            }

            var stadium = await _stadiumService.GetStadiumByIdAsync(stadiumId);

            if (stadium == null || stadium.OwnerId != user.Id)
            {
                return NotFound();
            }

            ViewBag.Stadium = stadium;

            return View(stadium.TimeSlots);
        }

        // GET: Owner/Bookings
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Bookings()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.Role != "Owner")
            {
                return RedirectToAction("Index", "Home");
            }

            var stadiums = await _stadiumService.GetStadiumsByOwnerIdAsync(user.Id);
            ViewBag.Stadiums = stadiums;

            // For now, return empty list (booking service not implemented yet)
            return View(new List<EhjozProject.Domain.Models.Booking.Booking>());
        }

        
    }
}
    

        
    

