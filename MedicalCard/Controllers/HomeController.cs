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
                return View(patients);
            }
            catch (System.Net.WebException)
            {
                return View();
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
                bundle.NextLink = new Uri(string.Format(BundleLinkString, nextPage));
            }
            if (!string.IsNullOrEmpty(previousPage))
            {
                bundle.PreviousLink = new Uri(string.Format(BundleLinkString, previousPage));
            }
            var nextPatients = GetPatientsToDisplay(bundle, goForward.Value);
            return View("Patient", nextPatients);
        }

        public IActionResult Search(string surname)
        {
            var resource = new ResourceGetter();
            if (Misc.DataValidator.TryValidateString(surname, out string validated, 2, 30))
            {
                ViewBag.Error = "Szukane nazwisko musi mieć między 2 a 30 znaków.";
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

        private void PutBundleInfoToViewBag(Bundle bundle)
        {
            if (bundle == null)
                return;

            TryGetPageRequestId(bundle.PreviousLink?.AbsoluteUri, out string pLink);
            TryGetPageRequestId(bundle.NextLink?.AbsoluteUri, out string nLink);

            ViewBag.bundle = new Dictionary<string, string>()
            {
                { "id", bundle.Id },
                { "previousPageLink", pLink },
                { "nextPageLink", nLink }
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

        const string BundleLinkString = @"http://localhost:8080/baseDstu3?_getpages={0}&_getpagesoffset=20&_count=20&_pretty=true&_bundletype=searchset";
        const string RequestIdIndicator = "_getpages=";
        const int PageIdLength = 36;
    }
}
