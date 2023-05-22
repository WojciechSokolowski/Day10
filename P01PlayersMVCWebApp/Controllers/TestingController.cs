using Microsoft.AspNetCore.Mvc;
using P01PlayersMVCWebApp.Models;

namespace P01PlayersMVCWebApp.Controllers
{
    public class TestingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SampleText()
        {
            ViewBag.MyOwnMessage = "hello from controler";
            ViewBag.Title = "Very nice site";
            return View();
        }

        public IActionResult PeronsList() 
        {
            var persons = new List<Person>()
            {
                new Person() { Id = 1, Name ="a",Age=1},
                new Person() { Id = 2, Name ="b",Age=2},
                new Person() { Id = 3, Name ="c",Age=3},
            };

            return View(persons);
        }

        public IActionResult NewPeronsList()
        {
            var persons = new List<Person>()
            {
                new Person() { Id = 1, Name ="a",Age=1},
                new Person() { Id = 2, Name ="b",Age=2},
                new Person() { Id = 3, Name ="c",Age=3},
            };

            return View(persons);
        }
    }
}
