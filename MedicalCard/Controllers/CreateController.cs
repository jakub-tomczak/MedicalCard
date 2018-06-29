using MedicalCard.Helpers;
using MedicalCard.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCard.Controllers
{
    public class CreateController : Controller
    {
        public IActionResult CreatePatient()
        {
            return View("CreatePatient");
        }
        [HttpPost]
        public IActionResult CreatePatient(EditedPatient patient)
        {
            if (ModelState.IsValid)
            {
                var client = new ResourceGetter();
                client.AddItem(patient.MapToResource());
                return RedirectToAction("InfoIndex", "Home", new { infoMessage = $"Dodano rekord {patient.GivenName} {patient.FamilyName}." });
            }
            else
            {
                return View(patient);
            }
        }
        [HttpPost]
        public IActionResult CreateObservation(EditedObservation observation)
        {
            return View();
        }
        public IActionResult CreateObservation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateMedication(EditedMedication observation)
        {
            return View();
        }
        public IActionResult CreateMedication()
        {
            return View();
        }
    }
}