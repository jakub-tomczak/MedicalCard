using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using MedicalCard.Helpers;
using MedicalCard.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCard.Controllers
{
    public class PresentationController : Controller
    {
        public IActionResult Index()
        {
            return View("ViewPatient");
        }
        public IActionResult ViewPatient(string id)
        {
            var resourceGetter = new ResourceGetter();
            var person = resourceGetter.GetItem<Patient>(id);
            var patientsObservations = resourceGetter.
                SearchItemsWithParameters<Observation>(
                    new List<Tuple<string, string>>() { new Tuple<string, string>("subject", id) }
                );
            var patientSummary = new PatientSummary()
            {
                Data = person,
                Observations = patientsObservations
            };
            return View(patientSummary);
        }
    }
}