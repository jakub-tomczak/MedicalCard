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
            var r = resource.GetItems<Patient>(20);

            var pat = r.FirstOrDefault();

            //pat.Name.Add(HumanName.ForFamily("Abrakadabra"));

            //resource.UpdateItem(pat);
            ViewBag.Patients = r;
            return View();
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

        public IActionResult EditPatient(string id)
        {
            var res = new ResourceGetter();
            var patient = res.GetItem<Patient>(ResourceType.Patient, id);
            ViewData["Patient"] = patient;
            return View(patient);
        }
        [HttpPost]
        public IActionResult EditPatient(Patient patient)
        {
            ViewBag["editStatus"] = "OK";
            return RedirectToAction("Patient");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
