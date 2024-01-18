using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebReservation.Data;
using WebReservation.Models;

namespace WebReservation.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly WebReservationContext _db;

        public Choice choice { get; set; }

        public Building building { get; set; }

        public AdminController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, WebReservationContext db)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _db = db;
        }

        private SelectList GetLocations()
        {
            return new SelectList(_db.Building.OrderBy(b => b.Name).ThenBy(r => r.Room),
                                  nameof(Building.FullName),
                                  nameof(Building.FullName));
        } 

        private SelectList GetUsers()
        {
            return new SelectList(_db.Users,
                                   nameof(IdentityUser.Email),
                                   nameof(IdentityUser.Email));
        }

        public List<Choice> GetAllChoices()
        {
            var choices = _db.Choice.OrderBy(d => d.DateChoice).ThenBy(d => d.Name).ToList();
            return choices;
        }

        public List<Choice> GetAllChoicesDay(DateTime day)
        {
            var keuzes = _db.Choice.Where(d => d.DateChoice == day).OrderBy(x => x.Location).ToList();

            return keuzes;
        }

        public int GetCapacity(string location)
        {
            var capacity = _db.Building.FirstOrDefault(b => b.FullName == location);
            return capacity.Capacity;
        }

        public IActionResult Reservations()
        {
            var reservations = GetAllChoices();
            ViewBag.reservations = reservations;

            return View();
        }

        [HttpGet]
        public IActionResult ReserveringenDay()
        {
            DateTime day = DateTime.Today;
            var reservations = GetAllChoicesDay(day);
            ViewBag.reservations = reservations;

            return View();
        }

        public IActionResult Day(string dateCheck)
        {
            if (dateCheck == null)
            {
                return RedirectToAction("ReserveringenDay");
            }
            else
            {
                DateTime test = DateTime.Parse(dateCheck);
                var reservations = GetAllChoicesDay(test);
                ViewBag.reservations = reservations;

                return View();
            }
        }

        public int CheckCapacity(string location, DateTime dateChoice)
        {
            var choices = _db.Choice.Where(l => l.Location == location).ToList();
            var keuzes2 = choices.Where(d => d.DateChoice == dateChoice).ToList().Count;

            return keuzes2;
        }

        public IActionResult CreateReservation()
        {
            ViewData["Users"] = GetUsers();
            ViewData["Locations"] = GetLocations();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReservation([Bind("Id,Name,Location,DateChoice,MaxCapacity")] Choice createChoice)
        {
            if (ModelState.IsValid)
            {
                int xchoices = CheckCapacity(createChoice.Location, createChoice.DateChoice);
                createChoice.MaxCapacity = GetCapacity(createChoice.Location);

                if (xchoices < createChoice.MaxCapacity)
                {
                    _db.Add(createChoice);
                    try
                    {
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Reservations");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = ex.Message;
                        ViewData["Locations"] = GetLocations();
                        ViewData["Users"] = GetUsers();
                        return View(createChoice);
                    }
                }

                else
                {
                    ViewBag.max = "lol";
                    ViewData["Locations"] = GetLocations();
                    ViewData["Users"] = GetUsers();
                    return View(createChoice);
                }
            }
            return View(createChoice);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Choice choice= _db.Choice.Single(c => c.Id == id);


            return View(choice);
        }

        [HttpPost]
        public ActionResult Delete(string Id)
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

        public List<IdentityUser> GetAllUsers()
        {
            var users = _db.Users.ToList();
            users.Sort((n1, n2) => string.Compare(n1.Email, n2.Email));

            return users;
        }

        public IActionResult Users()
        {
            var users = GetAllUsers();
            ViewBag.users = users;

            return View();
        }

        [HttpGet]
        public IActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserCreate(Register model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Users", "Admin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult UserDelete(string id)
        {
            IdentityUser user = _db.Users.SingleOrDefault(u => u.Id == id);

            return View(user);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.error = "lol";
            }
            else
            {
                _ = await userManager.DeleteAsync(user);
                ViewBag.message = "idk";
            }
            return View(user);
        }

        public List<Building> GetAllLocations()
        {
            var locations = _db.Building.ToList();
            locations.Sort((n1, n2) => string.Compare(n1.Name, n2.Name));

            return locations;
        }

        // locaties pagina
        public IActionResult Locations()
        {
            var locations = GetAllLocations();
            ViewBag.locations = locations;

            return View();
        }

        public IActionResult CreateLocation()
        {
            building = new Building();

            return View(building);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLocation(Building building)
        {
            building.FullName = building.Name + " " + building.Room;
            _db.Building.Add(building);
            _db.SaveChanges();
            return RedirectToAction("Locations");

            //modelstate gives false, why?
            //if (ModelState.IsValid)
            //{
            //    _db.Building.Add(building);
            //    _db.SaveChanges();
            //    return RedirectToAction("Locations");
            //}
            ////_db.Building.Add(building);
            ////_db.SaveChanges();
            //return RedirectToAction("Locations");
        }

        [HttpGet]
        public ActionResult DeleteLocation(int id)
        {
            Building building = _db.Building.Single(k => k.Id == id);

            return View(building);
        }

        [HttpPost]
        public ActionResult DeleteLocation(string Id)
        {
            int gebouwId = int.Parse(Id);
            Building building = _db.Building.SingleOrDefault(k => k.Id == gebouwId);

            if (building == null)
            {
                ViewBag.error = "lol";
            }
            else
            {
                _db.Building.Remove(building);
                _db.SaveChanges();
                ViewBag.message = "idk";
            }
            return View(building);
        }
    }
}
