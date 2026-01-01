using EhjozProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EhjozProject.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStadiumService _stadiumService;

        public HomeController(IStadiumService stadiumService)
        {
            _stadiumService = stadiumService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredStadiums = await _stadiumService.GetFeaturedStadiumsAsync(6);
            ViewBag.FeaturedStadiums = featuredStadiums;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}