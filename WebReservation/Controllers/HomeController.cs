using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebReservation.Data;
using WebReservation.Models;

namespace WebReservation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebReservationContext _db;

        public HomeController(ILogger<HomeController> logger, WebReservationContext db)
        {
            _logger = logger;
            _db = db;
        }

        public List<Choice> GetChoiceByName(string name)
        {
            var choices = _db.Choice.Where(k => k.Name == name).ToList();
            choices.Sort((o1, o2) => DateTime.Compare(o1.DateChoice, o2.DateChoice));
            return choices;
        }
        
        public IActionResult Index()
        {
            var choices = GetChoiceByName(User.Identity.Name);

            // check if the existing reservations are yet to come
            foreach (var k in choices.ToList())
            {
                var res = DateTime.Compare(k.DateChoice, DateTime.Now.AddDays(-1));
                // if older than today, delete it from view
                if (res == -1)
                {
                    choices.Remove(k);
                }
            }

            ViewBag.Choices = choices;
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}