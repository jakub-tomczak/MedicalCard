using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using MedicalCard.Helpers;
using MedicalCard.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MedicalCard.Controllers
{
    public class EditController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult EditPatient(string id)
        {
            var res = new ResourceGetter();
            var patient = res.GetItem<Patient>(id);
            var editablePatient = new EditedPatient(id);
            editablePatient.MapFromResource(patient);
            return View(editablePatient);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientEdited(EditedPatient patient)
        {
            if (ModelState.IsValid)
            {
                var editedPatient = patient.MapToResource();
                var client = new ResourceGetter();
                var result = client.UpdateItem(editedPatient);
            }
            else
            {
                return View("EditPatient", patient);
            }
            return RedirectToAction("Patient", "Home");
        }
    }
}