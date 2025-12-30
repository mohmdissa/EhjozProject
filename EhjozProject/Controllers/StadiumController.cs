using Microsoft.AspNetCore.Mvc;

namespace EhjozProject.Web.Controllers
{
    public class StadiumController : Controller
    {
        // Temporary test - no service needed
        public IActionResult Index()
        {
            return View();  // Just return empty view
        }
    }
}