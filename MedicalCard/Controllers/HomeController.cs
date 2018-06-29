using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using MedicalCard.Helpers;
using MedicalCard.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MedicalCard.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Patient()
        {
            var resource = new ResourceGetter();
            try
            {
                var patients = GetPatientsToDisplay();
                return View("Patient", patients);
            }
            catch (System.Net.WebException)
            {
                return View("Patient");
            }
        }

        public IActionResult ChangePage(string id, string nextPage, string previousPage, bool? goForward)
        {
            if (goForward == null || id == null || (nextPage == null && goForward.Value) || (previousPage == null && !goForward.Value))
            {
                return RedirectToAction("Patient");
            }
            var bundle = new Bundle
            {
                Id = id
            };
            if (!string.IsNullOrEmpty(nextPage))
            {
                bundle.NextLink = new Uri(string.Format(BundleRequest, nextPage));
            }
            if (!string.IsNullOrEmpty(previousPage))
            {
                bundle.PreviousLink = new Uri(string.Format(BundleRequest, previousPage));
            }
            var nextPatients = GetPatientsToDisplay(bundle, goForward.Value);
            return View("Patient", nextPatients);
        }

        public IActionResult Search(string surname)
        {
            var resource = new ResourceGetter();
            if (!Misc.DataValidator.TryValidateString(surname, out string validated, 2, 30))
            {
                return ErrorIndex("Szukane nazwisko musi mieć między 2 a 30 znaków.");
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

        public IActionResult ErrorIndex(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.errorMessage = errorMessage;
            }
            return Patient();
        }
        public IActionResult InfoIndex(string infoMessage)
        {
            if (!string.IsNullOrEmpty(infoMessage))
            {
                ViewBag.infoMessage = infoMessage;
            }
            return Patient();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Patient");

            var resource = new ResourceGetter();
            var person = resource.GetItem<Patient>(id);
            if (person == null)
                return RedirectToAction("Patient");
            if (resource.RemoveItem<Patient>(person))
                return InfoIndex($"Usunięto pacjenta {person.Name?.LastOrDefault()?.TypeName} o id {person.Id}");
            return ErrorIndex($"Wystąpił błąd podczas usuwania osoby {person.Name?.LastOrDefault()?.TypeName} o id {person.Id}");
        }

        private void PutBundleInfoToViewBag(Bundle bundle)
        {
            if (bundle == null)
                return;

            TryGetPageRequestParameters(bundle.NextLink?.AbsoluteUri, out string nextLink);
            TryGetPageRequestParameters(bundle.PreviousLink?.AbsoluteUri, out string previousLink);

            ViewBag.bundle = new Dictionary<string, string>()
            {
                { "id", bundle.Id },
                { "previousPageLink", previousLink },
                { "nextPageLink", nextLink }
            };
        }

        private IEnumerable<Patient> GetPatientsToDisplay(Bundle incomingBundle = null, bool goForward = true)
        {
            var resource = new ResourceGetter();
            try
            {
                IEnumerable<Patient> patients = new List<Patient>();
                if (incomingBundle == null)
                {
                    patients = resource.GetItems<Patient>(out Bundle bundle, limit: 20);
                    PutBundleInfoToViewBag(bundle);
                }
                else
                {
                    if (resource.TryGetPage(incomingBundle, (goForward ? PageDirection.Next : PageDirection.Previous), out Bundle outcomingBundle))
                    {
                        patients = outcomingBundle.Entry.Select(x => x.Resource).Cast<Patient>();
                        PutBundleInfoToViewBag(outcomingBundle);
                    }
                }
                return patients;// == null ? new List<Patient>() : patients; //return an empty list if 
            }
            catch (System.Net.WebException)
            {
                return null;
            }
        }

        private bool TryGetPageRequestId(string input, out string output)
        {
            output = string.Empty;
            if (string.IsNullOrEmpty(input))
                return false;

            var position = input.IndexOf(RequestIdIndicator) + RequestIdIndicator.Length;
            if (position == RequestIdIndicator.Length - 1 || input.Length < position + PageIdLength)
                return false;

            output = input.Substring(position, PageIdLength);
            return true;
        }

        private bool TryGetPageRequestParameters(string input, out string output)
        {
            output = string.Empty;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var parametersPosition = input.IndexOf(ParametersIntdicator) + ParametersIntdicator.Length;
            if (parametersPosition == parametersPosition - 1 || input.Length < parametersPosition + 50)
            {
                return false;
            }
            output = input.Substring(parametersPosition);
            return true;
        }

        const string BundleRequest = @"http://localhost:8080/baseDstu3?{0}";
        const string BundleLinkString = @"http://localhost:8080/baseDstu3?_getpages={0}&_getpagesoffset=20&_count=20&_pretty=true&_bundletype=searchset";
        const string RequestIdIndicator = "_getpages=";
        const string ParametersIntdicator = "baseDstu3?";
        const int PageIdLength = 36;
    }
}
