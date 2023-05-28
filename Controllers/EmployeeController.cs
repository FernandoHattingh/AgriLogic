using AgriLogic.Models;
using AgriLogic.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AgriLogic.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee/Index
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

        // GET: Employee/Create
        public IActionResult Create()
        {
            var model = new Farmer();
            model.ID = _context.Farmers.Max(f => f.ID) + 1; // Autopopulate the ID field
            return View(model);
        }

        // POST: Employee/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                // Get the logged-in farmer's username
                var loggedInUsername = User.FindFirstValue(ClaimTypes.Name);

                // Retrieve the logged-in farmer from the database based on the username
                var loggedInFarmer = _context.Farmers.FirstOrDefault(f => f.Username == loggedInUsername);

                // Retrieve the maximum ID from the database and increment it by 1
                int newId = _context.Farmers.Max(f => f.ID) + 1;

                var newFarmer = new Farmer
                {
                    ID = newId,
                    Username = farmer.Username,
                    Password = farmer.Password,
                    Name = farmer.Name,
                    Surname = farmer.Surname,
                    Product = farmer.Product // Use the product provided by the user
                };

                // Add the new farmer to the database
                _context.Farmers.Add(newFarmer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If the model state is invalid, return the form with the validation errors
            return View(farmer);
        }
    }
}
