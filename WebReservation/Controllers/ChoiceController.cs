using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebReservation.Data;
using WebReservation.Models;

namespace WebReservation.Controllers
{
    public class ChoiceController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly WebReservationContext _db;

        [BindProperty]
        public Choice choice { get; set; }

        public ChoiceController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, WebReservationContext db)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _db = db;
        }

        public List<Choice> GetChoiceByName(string name)
        {
            var choices = _db.Choice.Where(k => k.Name == name).ToList();
            choices.Sort((o1, o2) => DateTime.Compare(o1.DateChoice, o2.DateChoice));
            return choices;
        }

        private SelectList GetLocations()
        {
            return new SelectList(_db.Building.OrderBy(b => b.Name).ThenBy(r => r.Room),
                                  nameof(Building.FullName),
                                  nameof(Building.FullName));
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

            ViewBag.choices = choices;

            return View();
        }

        public int CheckCapacity(string x, DateTime y)
        {
            var choices = _db.Choice.Where(l => l.Location == x).ToList();
            var keuzes2 = choices.Where(d => d.DateChoice == y).ToList().Count;

            return keuzes2;
        }

        public IActionResult CreateReservation(int? id)
        {
            choice = new Choice();
            if (id == null)
            {
                ViewData["Locations"] = GetLocations();
                //create
                
                return View(choice);
            }

            return View(choice);
        }

        public int GetCapacity(string location)
        {
            var capacity = _db.Building.FirstOrDefault(b => b.FullName == location);
            return capacity.Capacity;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateReservation()
        {
            if (ModelState.IsValid)
            {
                if (choice.Id == 0)
                {   
                    int xChoices = CheckCapacity(choice.Location, choice.DateChoice);
                    //var lol = _db.Gebouws.Where(g => g.FullName == Keuze.KamerKeuze);
                    choice.MaxCapacity = GetCapacity(choice.Location);

                    var choices = GetChoiceByName(User.Identity.Name);
                    bool reservationExists = choices.Any(d => d.DateChoice == choice.DateChoice);

                    if (xChoices < choice.MaxCapacity && reservationExists == false)
                    {
                        _db.Choice.Add(choice);
                        try
                        {
                            _db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        catch (Exception)
                        {
                            ViewBag.error = "lol";
                            ViewData["Locations"] = GetLocations();
                            return View(choice);
                        }
                    }
                    else if (xChoices >= choice.MaxCapacity)
                    {
                        ViewBag.error = "lol";
                        ViewData["Locations"] = GetLocations();
                        return View(choice);
                    }
                    else
                    {
                        ViewBag.max = "lol";
                        ViewData["Locations"] = GetLocations();
                        return View(choice);
                    }
                }
            }
            return View(choice);
        }

        [HttpGet]
        public ActionResult DeleteReservation(int id)
        {
            Choice choice = _db.Choice.SingleOrDefault(c => c.Id == id);

            return View(choice);
        }

        [HttpPost]
        public ActionResult DeleteReservation(string Id)
        {
            int choiceId = int.Parse(Id);
            Choice choice = _db.Choice.SingleOrDefault(c => c.Id == choiceId);

            if (choice == null)
            {
                ViewBag.error = "lol";
            }
            else
            {
                _db.Choice.Remove(choice);
                _db.SaveChanges();
                ViewBag.message = "idk";
            }

            return View(choice);
        }
    }
}
