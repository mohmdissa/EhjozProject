using EhjozProject.Application.Interfaces;
using EhjozProject.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EhjozProject.Web.Controllers
{
    public class StadiumController : Controller
    {
        private readonly IStadiumService _stadiumService;

        public StadiumController(IStadiumService stadiumService)
        {
            _stadiumService = stadiumService;
        }

        // GET: Stadium
        public async Task<IActionResult> Index(string? city, decimal? maxPrice, string? q)
        {
            var stadiums = await _stadiumService.GetAllStadiumsAsync();

            // Apply filters
            if (!string.IsNullOrEmpty(city))
            {
                stadiums = stadiums.Where(s => s.City.ToLower() == city.ToLower()).ToList();
                ViewBag.SelectedCity = city;
            }

            if (maxPrice.HasValue)
            {
                stadiums = stadiums.Where(s => s.PricePerHour <= maxPrice.Value).ToList();
                ViewBag.SelectedMaxPrice = maxPrice.Value;
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var needle = q.Trim().ToLower();
                stadiums = stadiums.Where(s =>
                        (s.Name != null && s.Name.ToLower().Contains(needle)) ||
                        (s.City != null && s.City.ToLower().Contains(needle)) ||
                        (s.Address != null && s.Address.ToLower().Contains(needle)))
                    .ToList();
                ViewBag.SearchQuery = q;
            }

            // If request is AJAX, return only the results partial (better UX, no full refresh)
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                        string.Equals(Request.Query["ajax"], "1", StringComparison.OrdinalIgnoreCase);
            if (isAjax)
            {
                return PartialView("_StadiumResults", stadiums);
            }

            return View(stadiums);
        }

        // GET: Stadium/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var stadium = await _stadiumService.GetStadiumByIdAsync(id);

            if (stadium == null)
            {
                return NotFound();
            }

            return View(stadium);
        }

        // GET: Stadium/Search
        public async Task<IActionResult> Search(string? city, DateTime? date, string? time)
        {
            var stadiums = await _stadiumService.GetAllStadiumsAsync();

            // Filter by city
            if (!string.IsNullOrEmpty(city))
            {
                stadiums = stadiums.Where(s => s.City.ToLower() == city.ToLower()).ToList();
            }

            ViewBag.SearchCity = city;
            ViewBag.SearchDate = date;
            ViewBag.SearchTime = time;

            return View("Index", stadiums);
        }
    }
}