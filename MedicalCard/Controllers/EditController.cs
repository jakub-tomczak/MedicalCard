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
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Patient", "Home");
            var res = new ResourceGetter();
            var patient = res.GetItem<Patient>(id);
            if (patient == null)
                return RedirectToAction("Patient", "Home");
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
                var client = new ResourceGetter();
                if (string.IsNullOrEmpty(patient.Id))
                {
                    return RedirectToAction("Patient", "Home");
                }
                //fetch original patient
                var originalPatient = client.GetItem<Patient>(patient.Id);
                if (originalPatient != null)
                {
                    if (patient.TryMergeWithResource(originalPatient))
                        client.UpdateItem(originalPatient);
                }

            }
            else
            {
                return View("EditPatient", patient);
            }
            return RedirectToAction("Patient", "Home");
        }
    }
}