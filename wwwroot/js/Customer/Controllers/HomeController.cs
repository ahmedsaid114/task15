using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Movies() => View();
        public IActionResult Cinemas() => View();
        public IActionResult About() => View();
        public IActionResult Details(int id = 1)
        {
            ViewBag.MovieId = id;
            return View();
        }
        public IActionResult Booking(int id = 1)
        {
            ViewBag.MovieId = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Booking(string movieTitle, string showDate, string showTime, string[] seats)
        {
            ViewBag.MovieTitle = movieTitle;
            ViewBag.ShowDate = showDate;
            ViewBag.ShowTime = showTime;
            ViewBag.Seats = seats ?? Array.Empty<string>();
            return View("Success");
        }
        public IActionResult Success() => View();
    }
}
