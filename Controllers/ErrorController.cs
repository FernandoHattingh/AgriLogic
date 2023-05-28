using AgriLogic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgriLogic.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var model = new ErrorViewModel(requestId);
            return View(model);
        }
    }
}
