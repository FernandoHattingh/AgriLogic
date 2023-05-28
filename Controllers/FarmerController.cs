using AgriLogic.Data;
using AgriLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgriLogic.Controllers
{
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FarmerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Farmer/Index
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
            var farmers = _context.Farmers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                farmers = farmers.Where(f =>
                    f.Username.Contains(searchString) ||
                    f.Password.Contains(searchString) ||
                    f.Name.Contains(searchString) ||
                    f.Surname.Contains(searchString) ||
                    f.Product.Contains(searchString));
            }

            return View(await farmers.ToListAsync());
        }

        // GET: Farmer/Create
        [Authorize(Policy = "FarmerOnly")]
        public IActionResult Create()
        {
            // Get the logged-in farmer's username
            var loggedInUsername = User.Identity.Name;

            // Retrieve the logged-in farmer from the database based on the username
            var loggedInFarmer = _context.Farmers.FirstOrDefault(f => f.Username == loggedInUsername);

            if (loggedInFarmer == null)
            {
                // Handle the case when no logged-in farmer is found
                return RedirectToAction("Index", "Home");
            }

            // Pre-populate the farmer's information in the create form
            var farmer = new Farmer
            {
                Username = loggedInFarmer.Username,
                Password = loggedInFarmer.Password,
                Name = loggedInFarmer.Name,
                Surname = loggedInFarmer.Surname
            };

            return View(farmer);
        }


        // POST: Farmer/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "FarmerOnly")]
        public async Task<IActionResult> Create(Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                // Get the logged-in farmer's username
                var loggedInUsername = User.Identity.Name;

                // Retrieve the logged-in farmer from the database based on the username
                var loggedInFarmer = _context.Farmers.FirstOrDefault(f => f.Username == loggedInUsername);

                var lastFarmer = await _context.Farmers.OrderByDescending(f => f.ID).FirstOrDefaultAsync();

                int newId = 1; // Default ID if no farmer exists yet

                if (lastFarmer != null)
                    newId = lastFarmer.ID + 1;

                var newFarmer = new Farmer
                {
                    ID = newId,
                    Username = loggedInFarmer.Username,
                    Password = loggedInFarmer.Password,
                    Name = loggedInFarmer.Name,
                    Surname = loggedInFarmer.Surname,
                    Product = farmer.Product
                };

                // Add the new farmer to the database
                _context.Farmers.Add(newFarmer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If the logged-in farmer is not found or the model state is invalid, return an error
            ModelState.AddModelError("", "Invalid farmer details.");

            return View(farmer);
        }
    }
}
