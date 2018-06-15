using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MedicalCard.Models;
using System.Xml.Linq;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using MedicalCard.Helpers;

namespace MedicalCard.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Patient()
        {
            var resource = new ResourceGetter();
            return View(resource.GetItems<Patient>(20));

        }

        public IActionResult Search(string surname)
        {
            var resource = new ResourceGetter();
            if (string.IsNullOrEmpty(surname))
            {
                ModelState.AddModelError("", "Surname is required");
                return Patient();
            }
            else
            {
                return View(
                    "Patient",
                    resource.SearchItemsWithParameters<Patient>(
                        new List<Tuple<string, string>>() { new Tuple<string, string>("family:contains", surname) }
                    ));
            }

        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
