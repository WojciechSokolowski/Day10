using Microsoft.AspNetCore.Mvc;

namespace P01PlayersMVCWebApp.Controllers
{
    public class CalculatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Add(IFormCollection from)
        {
            int number1 = int.Parse(from["number1"]);
            int number2 = int.Parse(from["number2"]);

            int result = number1 + number2;

            ViewBag.Result = result;

            return View("Index");
        }

    }
}
