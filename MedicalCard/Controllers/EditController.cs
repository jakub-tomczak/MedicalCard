using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using MedicalCard.Helpers;
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
            return View(patient);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientEdited(string id, string familyname)
        {
            var formData = HttpContext.Request.Form;
            var serialized = JsonConvert.SerializeObject(formData);
            var res = new ResourceGetter();
            var patient = res.GetItems<Patient>().FirstOrDefault();

            var b = new FhirJsonSerializer();
            var strBuilder = new StringBuilder();
            b.Serialize(patient, new JsonTextWriter(new StringWriter(strBuilder)));


            var a = new FhirJsonParser();
            var newPatient = a.Parse<Patient>(new JsonTextReader(new StringReader(strBuilder.ToString())));
            return RedirectToAction("Patient", "Home");
        }
    }
}