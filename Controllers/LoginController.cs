using AgriLogic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgriLogic.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            ViewData["HideNavbar"] = true;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the home page or any other appropriate page
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel model)
        {
            string con = "Server=tcp:fernandosr.database.windows.net,1433; Initial Catalog=AgriLogic; Authentication=Active Directory Password; User ID=ST10115510@vcconnect.edu.za; Password=Fromlive53;";

            using (SqlConnection connection = new SqlConnection(con))
            {
                if (ModelState.IsValid)
                {
                    if (model.Role == "Farmer")
                    {
                        using (SqlCommand comm = new SqlCommand("SELECT * FROM dbo.Farmer WHERE dbo.Farmer.Username = @Username AND dbo.Farmer.Password = @Password", connection))
                        {
                            comm.Parameters.AddWithValue("@Username", model.Username);
                            comm.Parameters.AddWithValue("@Password", model.Password);
                            connection.Open();
                            SqlDataReader reader = comm.ExecuteReader();
                            if (reader.Read())
                            {
                                var farmerId = reader.GetInt32(reader.GetOrdinal("ID"));
                                connection.Close();

                                // Create the claims for the authenticated user
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, model.Username),
                                    new Claim(ClaimTypes.Role, "Farmer"),
                                    new Claim("FarmerId", farmerId.ToString())
                                };

                                // Create the identity for the claims
                                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                var principal = new ClaimsPrincipal(identity);

                                // Perform the authentication process
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                                // Return a JSON response with the role information
                                return Json(new { role = "Farmer" });
                            }
                        }
                    }
                    else if (model.Role == "Employee")
                    {
                        using (SqlCommand comm = new SqlCommand("SELECT * FROM dbo.Employee WHERE dbo.Employee.Username = @Username AND dbo.Employee.Password = @Password", connection))
                        {
                            comm.Parameters.AddWithValue("@Username", model.Username);
                            comm.Parameters.AddWithValue("@Password", model.Password);
                            connection.Open();
                            SqlDataReader reader = comm.ExecuteReader();
                            if (reader.Read())
                            {
                                connection.Close();

                                // Create the claims for the authenticated user
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, model.Username),
                                    new Claim(ClaimTypes.Role, "Employee")
                                };

                                // Create the identity for the claims
                                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                var principal = new ClaimsPrincipal(identity);

                                // Perform the authentication process
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                                // Return a JSON response with the role information
                                return Json(new { role = "Employee" });
                            }
                        }
                    }
                }
            }

            // Invalid login, return a JSON response with an error message
            return Json(new { errorMessage = "Invalid login credentials." });
        }
    }
}
